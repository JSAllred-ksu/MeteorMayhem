using GameArchitectureExample.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct2D1;
using System;
using System.Reflection.Metadata;

namespace GameArchitectureExample.Screens
{
    public class VictoryScreen : MenuScreen
    {
        private readonly TimeSpan _gameplayDuration;
        private ContentManager _content;
        private Texture2D _background;
        private string _victoryMessage;
        private int _asteroidsDestroyed;
        private bool _isTimeTrial;

        public VictoryScreen(int asteroidsDestroyed) : base("Game Over")
        {
            _asteroidsDestroyed = asteroidsDestroyed;
            _isTimeTrial = false;
            InitializeMenuEntries();
        }

        public VictoryScreen(TimeSpan activeTime) : base("Time Trial Complete")
        {
            _gameplayDuration = activeTime;
            _isTimeTrial = true;
            InitializeMenuEntries();
        }

        private void InitializeMenuEntries()
        {
            var returnToTitleMenuEntry = new MenuEntry("Return to Title");
            returnToTitleMenuEntry.Selected += ReturnToTitleMenuEntrySelected;

            MenuEntries.Add(returnToTitleMenuEntry);
        }

        private void ReturnToTitleMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MediaPlayer.Stop();
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }

        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("Nebula");

            IsPopup = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);

            // Draw background
            ScreenManager.SpriteBatch.Draw(_background, new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), Color.White);

            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);

            // Prepare message based on game mode
            string primaryMessage;
            if (_isTimeTrial)
            {
                primaryMessage = $"Time Taken: {_gameplayDuration.Minutes:D2}:{_gameplayDuration.Seconds:D2}.{_gameplayDuration.Milliseconds:D3}";
            }
            else
            {
                primaryMessage = $"Asteroids Destroyed: {_asteroidsDestroyed}";
            }

            // Measure and draw primary message
            Vector2 messageSize = ScreenManager.Font.MeasureString(primaryMessage);
            Vector2 messagePosition = new Vector2((viewportSize.X - messageSize.X) / 2, 220);

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, primaryMessage, messagePosition, Color.White);

            ScreenManager.SpriteBatch.End();

            // Draw menu entries
            base.Draw(gameTime);
        }
    }
}