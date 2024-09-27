using GameArchitectureExample.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameArchitectureExample.Screens
{
    public class VictoryScreen : GameScreen
    {
        private readonly TimeSpan _gameplayDuration;
        private ContentManager _content;
        private Texture2D _background;
        private string _victoryMessage;
        private Vector2 _messagePosition;
        private float _scale = 1f;
        private float _timeScale = 1.2f;
        private Vector2 _timePosition;
        private float _padding = 50f;

        public VictoryScreen(TimeSpan gameplayDuration)
        {
            _gameplayDuration = gameplayDuration;
            IsPopup = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("Nebula");
            _victoryMessage = "Congratulations! You saved the galaxy!";

            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);

            // center victory message
            var messageSize = ScreenManager.Font.MeasureString(_victoryMessage) * _scale;
            _messagePosition = new Vector2(_padding, (viewportSize.Y - messageSize.Y * _scale) / 2);

            // message scale since too wide for screen
            if (messageSize.X > viewport.Width - 2 * _padding)
            {
                _scale = (viewport.Width - 2 * _padding) / messageSize.X;
                messageSize = ScreenManager.Font.MeasureString(_victoryMessage) * _scale;
                _messagePosition.Y = (viewportSize.Y - messageSize.Y) / 2;
            }

            // center time message just below victory message
            string timeMessage = _gameplayDuration.ToString(@"mm\:ss");
            var timeSize = ScreenManager.Font.MeasureString(timeMessage) * _timeScale;
            _timePosition = new Vector2(
                (viewportSize.X - timeSize.X * 2) / 2, // center
                _messagePosition.Y + messageSize.Y + 5f // position below 
            );
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
            ScreenManager.SpriteBatch.Draw(_background, new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), Color.White);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, _victoryMessage, _messagePosition, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, $"Time: {_gameplayDuration.ToString(@"mm\:ss")}", _timePosition, Color.White, 0f, Vector2.Zero, _timeScale, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.End();
        }
    }
}