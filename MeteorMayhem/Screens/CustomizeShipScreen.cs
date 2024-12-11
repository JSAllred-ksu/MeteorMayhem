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

        private Vector2 _shipPosition;
        private float _shipScale = 12.0f;

        private Texture2D _colorWheel;
        private Texture2D _colorWheelCursor;
        private Rectangle _colorWheelRect;
        private Vector2 _colorWheelCenter;
        private float _colorWheelRadius;

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

            _previewShip = new ShipSprite(ScreenManager.Game);
            _previewShip.LoadContent(_content);
            _shipPosition = new Vector2(
                ScreenManager.GraphicsDevice.Viewport.Width * 0.25f,
                ScreenManager.GraphicsDevice.Viewport.Height * 0.5f
            );

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

            _pixel = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            CreateColorWheel();
            _colorWheelCursor = new Texture2D(ScreenManager.GraphicsDevice, 10, 10);
            Color[] cursorData = new Color[100];
            for (int i = 0; i < 100; i++)
            {
                cursorData[i] = Color.White;
            }
            _colorWheelCursor.SetData(cursorData);
        }

        private void CreateColorWheel()
        {
            int wheelSize = 400;
            _colorWheelRadius = wheelSize / 2;
            _colorWheel = new Texture2D(ScreenManager.GraphicsDevice, wheelSize, wheelSize);
            Color[] colorData = new Color[wheelSize * wheelSize];

            Vector2 center = new Vector2(wheelSize / 2f);
            float radius = wheelSize / 2f;

            for (int x = 0; x < wheelSize; x++)
            {
                for (int y = 0; y < wheelSize; y++)
                {
                    Vector2 pixel = new Vector2(x, y);
                    Vector2 delta = pixel - center;
                    float distance = delta.Length();

                    if (distance <= radius)
                    {
                        float angle = (float)Math.Atan2(delta.Y, delta.X);
                        float hue = (angle + MathHelper.Pi) / MathHelper.TwoPi;
                        float saturation = distance / radius;

                        colorData[y * wheelSize + x] = HsvToRgb(hue, saturation, 1f);
                    }
                    else
                    {
                        colorData[y * wheelSize + x] = Color.Transparent;
                    }
                }
            }

            _colorWheel.SetData(colorData);

            // wheel to right side of screen
            _colorWheelRect = new Rectangle(
                (int)(ScreenManager.GraphicsDevice.Viewport.Width * 0.7f - _colorWheelRadius),
                (int)(ScreenManager.GraphicsDevice.Viewport.Height * 0.5f - _colorWheelRadius),
                wheelSize,
                wheelSize
            );
            _colorWheelCenter = new Vector2(_colorWheelRect.Center.X, _colorWheelRect.Center.Y);
        }

        private Color HsvToRgb(float h, float s, float v)
        {
            float c = v * s;
            float x = c * (1 - Math.Abs((h * 6) % 2 - 1));
            float m = v - c;

            float r, g, b;
            if (h < 1f / 6f) { r = c; g = x; b = 0; }
            else if (h < 2f / 6f) { r = x; g = c; b = 0; }
            else if (h < 3f / 6f) { r = 0; g = c; b = x; }
            else if (h < 4f / 6f) { r = 0; g = x; b = c; }
            else if (h < 5f / 6f) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return new Color(r + m, g + m, b + m);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null) return;

            MouseState mouseState = Mouse.GetState();
            bool clicked = mouseState.LeftButton == ButtonState.Pressed;

            if (clicked)
            {
                Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

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
                else if (Vector2.Distance(mousePos, _colorWheelCenter) <= _colorWheelRadius)
                {
                    Vector2 delta = mousePos - _colorWheelCenter;
                    float angle = (float)Math.Atan2(delta.Y, delta.X);
                    float distance = delta.Length();
                    float hue = (angle + MathHelper.Pi) / MathHelper.TwoPi;
                    float saturation = Math.Min(distance / _colorWheelRadius, 1f);

                    _selectedColor = HsvToRgb(hue, saturation, 1f);
                    _previewShip.Color = _selectedColor;
                }
            }
        }

        private void SaveSelectedColor(Color color)
        {
            var gameState = ScreenManager.Game.Services.GetService<GameState>();
            if (gameState == null)
            {
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

            // ship scaled and positioned
            _spriteBatch.Draw(
                _previewShip.Texture,
                _shipPosition,
                new Rectangle(
                    (_previewShip.Texture.Width - _previewShip.FrameWidth) / 2,
                    0,
                    _previewShip.FrameWidth,
                    _previewShip.FrameHeight
                ),
                _selectedColor,
                0f,
                new Vector2(_previewShip.FrameWidth / 2f, _previewShip.FrameHeight / 2f),
                _shipScale,
                SpriteEffects.None,
                0f
            );

            // color wheel
            _spriteBatch.Draw(_colorWheel, _colorWheelRect, Color.White);

            if (_selectedColor != Color.White)
            {
                // cursor position based on selected color
                RgbToHsv(_selectedColor, out float h, out float s, out float v);
                float angle = h * MathHelper.TwoPi - MathHelper.Pi;
                float distance = s * _colorWheelRadius;
                Vector2 cursorPos = _colorWheelCenter + new Vector2(
                    (float)Math.Cos(angle) * distance,
                    (float)Math.Sin(angle) * distance
                );
                _spriteBatch.Draw(_colorWheelCursor,
                    new Rectangle((int)cursorPos.X - 5, (int)cursorPos.Y - 5, 10, 10),
                    Color.White);
            }

            // buttons
            _spriteBatch.Draw(_buttonTexture, _saveButton, Color.DarkGray);
            _spriteBatch.Draw(_buttonTexture, _cancelButton, Color.DarkGray);

            Vector2 savePos = new Vector2(_saveButton.Center.X - _font.MeasureString("Save").X / 2,
                _saveButton.Center.Y - _font.MeasureString("Save").Y / 2);
            Vector2 backPos = new Vector2(_cancelButton.Center.X - _font.MeasureString("Back").X / 2,
                _cancelButton.Center.Y - _font.MeasureString("Back").Y / 2);

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

        private void RgbToHsv(Color color, out float h, out float s, out float v)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            // value
            v = max;

            // saturation
            s = max == 0 ? 0 : delta / max;

            // hue
            if (delta == 0)
            {
                h = 0;
            }
            else if (max == r)
            {
                h = ((g - b) / delta) % 6;
            }
            else if (max == g)
            {
                h = 2f + (b - r) / delta;
            }
            else
            {
                h = 4f + (r - g) / delta;
            }

            h /= 6f;
            if (h < 0)
                h += 1f;
        }

        public override void Unload()
        {
            _content.Unload();
            _pixel.Dispose();
            _colorWheel.Dispose();
            _colorWheelCursor.Dispose();
            _buttonTexture.Dispose();
        }
    }
}
