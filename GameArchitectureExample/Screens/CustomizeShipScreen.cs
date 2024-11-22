using GameArchitectureExample.StateManagement;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace GameArchitectureExample.Screens
{
    public class CustomizeShipScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private ShipSprite _previewShip;
        private Texture2D _pixel;

        private Rectangle[] _colorButtons;
        private Color[] _colorOptions;
        private Color _selectedColor = Color.White;

        private readonly InputAction _backAction;

        private Rectangle _saveButton;
        private Rectangle _cancelButton;
        private Texture2D _buttonTexture;
        private SpriteFont _font;

        private bool _showSaveMessage;
        private float _messageTimer;
        private const float MESSAGE_DURATION = 2f;

        public CustomizeShipScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _backAction = new InputAction(
                new[] { Buttons.Back, Buttons.B },
                new[] { Keys.Back, Keys.Escape }, true);
        }

        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _spriteBatch = ScreenManager.SpriteBatch;
            _font = _content.Load<SpriteFont>("gamefont");

            int buttonWidth = 120;
            int buttonHeight = 40;
            int buttonY = ScreenManager.GraphicsDevice.Viewport.Height - 50;

            _saveButton = new Rectangle(
                ScreenManager.GraphicsDevice.Viewport.Width / 2 - buttonWidth - 10,
                buttonY,
                buttonWidth,
                buttonHeight
            );

            _cancelButton = new Rectangle(
                ScreenManager.GraphicsDevice.Viewport.Width / 2 + 10,
                buttonY,
                buttonWidth,
                buttonHeight
            );

            _buttonTexture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            _buttonTexture.SetData(new[] { Color.DarkViolet });

            _previewShip = new ShipSprite(ScreenManager.Game);
            _previewShip.LoadContent(_content);

            _pixel = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _colorOptions = new Color[]
            {
                Color.White,
                Color.Red,
                Color.Blue,
                Color.Green,
                Color.Yellow,
                Color.Purple,
                Color.Orange,
                Color.Cyan
            };

            // Create color selection buttons
            int buttonSize = 40;
            int spacing = 10;
            int startX = (ScreenManager.GraphicsDevice.Viewport.Width - (_colorOptions.Length * (buttonSize + spacing))) / 2;
            int startY = ScreenManager.GraphicsDevice.Viewport.Height - 100;

            _colorButtons = new Rectangle[_colorOptions.Length];
            for (int i = 0; i < _colorOptions.Length; i++)
            {
                _colorButtons[i] = new Rectangle(
                    startX + (i * (buttonSize + spacing)),
                    startY,
                    buttonSize,
                    buttonSize
                );
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null) return;

            MouseState mouseState = Mouse.GetState();
            bool clicked = mouseState.LeftButton == ButtonState.Pressed;

            if (clicked)
            {
                if (_saveButton.Contains(mouseState.Position))
                {
                    SaveSelectedColor(_selectedColor);
                    _showSaveMessage = true;
                    _messageTimer = MESSAGE_DURATION;
                }
                else if (_cancelButton.Contains(mouseState.Position))
                {
                    ExitScreen();
                    return;
                }
                else
                {
                    for (int i = 0; i < _colorButtons.Length; i++)
                    {
                        if (_colorButtons[i].Contains(mouseState.Position))
                        {
                            _selectedColor = _colorOptions[i];
                            _previewShip.ShipColor = _selectedColor;
                            break;
                        }
                    }
                }
            }
        }

        private void SaveSelectedColor(Color color)
        {
            var gameState = ScreenManager.Game.Services.GetService<GameState>();
            if (gameState == null)
            {
                // For debugging - helps identify if GameState service is missing
                System.Diagnostics.Debug.WriteLine("GameState service not found!");
                return;
            }
            gameState.ShipColor = color;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (_showSaveMessage)
            {
                _messageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_messageTimer <= 0)
                {
                    _showSaveMessage = false;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var viewport = ScreenManager.GraphicsDevice.Viewport;

            _spriteBatch.Begin();

            _previewShip.Draw(gameTime, _spriteBatch);

            for (int i = 0; i < _colorButtons.Length; i++)
            {
                _spriteBatch.Draw(_pixel, _colorButtons[i], _colorOptions[i]);
                if (_colorOptions[i] == _selectedColor)
                {
                    var borderRect = _colorButtons[i];
                    borderRect.Inflate(2, 2);
                    _spriteBatch.Draw(_pixel, new Rectangle(borderRect.X, borderRect.Y, borderRect.Width, 2), Color.White);
                    _spriteBatch.Draw(_pixel, new Rectangle(borderRect.X, borderRect.Y + borderRect.Height - 2, borderRect.Width, 2), Color.White);
                    _spriteBatch.Draw(_pixel, new Rectangle(borderRect.X, borderRect.Y, 2, borderRect.Height), Color.White);
                    _spriteBatch.Draw(_pixel, new Rectangle(borderRect.X + borderRect.Width - 2, borderRect.Y, 2, borderRect.Height), Color.White);
                }
            }

            _spriteBatch.Draw(_buttonTexture, _saveButton, Color.DarkGray);
            _spriteBatch.Draw(_buttonTexture, _cancelButton, Color.DarkGray);

            Vector2 savePos = new Vector2(_saveButton.Center.X - _font.MeasureString("Save").X / 2, _saveButton.Center.Y - _font.MeasureString("Save").Y / 2);
            Vector2 backPos = new Vector2(_cancelButton.Center.X - _font.MeasureString("Back").X / 2, _cancelButton.Center.Y - _font.MeasureString("Back").Y / 2);

            _spriteBatch.DrawString(_font, "Save", savePos, Color.White);
            _spriteBatch.DrawString(_font, "Back", backPos, Color.White);

            if (_showSaveMessage)
            {
                string message = "Color saved!";
                Vector2 messageSize = _font.MeasureString(message);
                Vector2 messagePos = new Vector2(
                    viewport.Width / 2 - messageSize.X / 2,
                    viewport.Height / 2 - messageSize.Y / 2);

                _spriteBatch.DrawString(_font, message, messagePos, Color.White);
            }

            _spriteBatch.End();
        }

        public override void Unload()
        {
            _content.Unload();
            _pixel.Dispose();
        }
    }
}
