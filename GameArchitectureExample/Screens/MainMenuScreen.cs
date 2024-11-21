using Microsoft.Xna.Framework;
using GameArchitectureExample.StateManagement;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameArchitectureExample.Screens
{
    // The main menu screen is the first thing displayed when the game starts up.
    public class MainMenuScreen : MenuScreen
    {
        private ContentManager _content;
        //private Model _asteroid;
        //private Texture2D _rockTexture; null reference every time ://
        private float _rotationY = 0f;
        private float _rotationX = 0f;
        // First asteroid position
        private Vector3 _asteroidPosition1 = new Vector3(15, 10, -10);
        // Second asteroid position (opposite side)
        private Vector3 _asteroidPosition2 = new Vector3(15, 10, 10);
        private Matrix _view = Matrix.CreateLookAt(
            new Vector3(-20, -20, 0),  // Camera position adjusted to see both asteroids
            Vector3.Zero,              // Look at point
            Vector3.Up                 // Up vector
        );
        private Matrix _projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,    // 45 degree field of view
            16f / 9f,              // Aspect ratio
            0.1f,                  // Near plane
            100f                   // Far plane
        );

        public MainMenuScreen() : base("Meteor Mayhem")
        {
            var playGameMenuEntry = new MenuEntry("New Game");
            var loadGameMenuEntry = new MenuEntry("Load Game");
            var trialGameMenuEntry = new MenuEntry("Galactic Trials");
            var exitMenuEntry = new MenuEntry("Exit");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            loadGameMenuEntry.Selected += LoadGameMenuEntrySelected;
            trialGameMenuEntry.Selected += TimeTrialsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(loadGameMenuEntry);
            MenuEntries.Add(trialGameMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        private void TimeTrialsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new TimeTrialsMenuScreen(), e.PlayerIndex);
        }

        private void LoadGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (File.Exists("save.json"))
            {
                var gameplayScreen = new GameplayScreen();
                string jsonString = File.ReadAllText("save.json");
                if (string.IsNullOrEmpty(jsonString))
                {
                    var messageBox = new MessageBoxScreen("Save file is empty or corrupted.");
                    ScreenManager.AddScreen(messageBox, e.PlayerIndex);
                    return;
                }

                GameState state = JsonSerializer.Deserialize<GameState>(jsonString);
                LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, gameplayScreen);
                gameplayScreen.LoadState(state);
            }
            else
            {
                var messageBox = new MessageBoxScreen("No saved game found.");
                ScreenManager.AddScreen(messageBox, e.PlayerIndex);
            }
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }

        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            //_asteroid = _content.Load<Model>("rock");
            //_rockTexture = _content.Load<Texture2D>("rock-texture-surface");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _rotationY += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _rotationX += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Matrix world1 = Matrix.CreateScale(0.2f) *
                          Matrix.CreateRotationY(_rotationY) *
                          Matrix.CreateRotationX(_rotationX) *
                          Matrix.CreateTranslation(_asteroidPosition1);

            Matrix world2 = Matrix.CreateScale(0.2f) *
                          Matrix.CreateRotationY(-_rotationY) *
                          Matrix.CreateRotationX(-_rotationX) *
                          Matrix.CreateTranslation(_asteroidPosition2);

            //foreach (var mesh in _asteroid.Meshes)
            //{
            //    foreach (var part in mesh.MeshParts)
            //    {
            //        part.Effect.Parameters["DiffuseTexture"].SetValue(_rockTexture);
            //    }
            //}
            //_asteroid.Draw(world1, _view, _projection);
            //_asteroid.Draw(world2, _view, _projection);
        }
    }
}