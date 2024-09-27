using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameArchitectureExample.StateManagement;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Media;

namespace GameArchitectureExample.Screens
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private readonly InputAction _pauseAction;
        private TimeSpan _activeTime;

        private SpriteBatch spriteBatch;
        private ShipSprite ship;
        private Texture2D background;
        private AsteroidSprite[] asteroids;
        private Song music;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back }, true);
        }

        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            ContentManager content = ScreenManager.Game.Content;
            spriteBatch = ScreenManager.SpriteBatch;

            GraphicsDevice graphicsDevice = ScreenManager.GraphicsDevice;
            Viewport viewport = graphicsDevice.Viewport;

            background = content.Load<Texture2D>("Nebula");
            music = content.Load<Song>("Voxel Revolution");
            MediaPlayer.Play(music);
            MediaPlayer.IsRepeating = true;

            System.Random rand = new System.Random();
            asteroids = new AsteroidSprite[10];
            for (int i = 0; i < asteroids.Length; i++)
            {
                float randomAngularSpeed = (float)(rand.NextDouble());
                int textureWidth = 96; 
                int textureHeight = 96; 

                Vector2 randomPosition = new Vector2(
                    (float)rand.NextDouble() * (viewport.Width - textureWidth),
                    (float)rand.NextDouble() * (viewport.Height - textureHeight));

                asteroids[i] = new AsteroidSprite(randomPosition, randomAngularSpeed);
                asteroids[i].LoadContent(content);
            }

            ship = new ShipSprite(ScreenManager.Game);
            ship.LoadContent(content);
        }

        public override void Unload()
        {
            _content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                _activeTime += gameTime.ElapsedGameTime;
            }

            if (!IsActive)
            {
                return;
            }

            for (int i = 0; i < asteroids.Length; i++)
            {
                if (asteroids[i] != null && !asteroids[i].Destroyed)
                {
                    asteroids[i].Update(gameTime, ship);
                }
                else if (asteroids[i]?.Destroyed == true)
                {
                    asteroids[i] = null;
                }
            }

            if (asteroids.All(a => a == null))
            {
                LoadingScreen.Load(ScreenManager, true, null, new VictoryScreen(_activeTime));
            }

            ship.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphicsDevice = ScreenManager.GraphicsDevice;
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);
            ship.Draw(gameTime, spriteBatch);

            foreach (var asteroid in asteroids.Where(a => a != null))
            {
                asteroid.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
        }
    }
}