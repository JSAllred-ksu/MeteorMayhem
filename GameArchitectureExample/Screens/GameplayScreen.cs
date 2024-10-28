using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameArchitectureExample.StateManagement;
using System;
using System.Collections.Generic;
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
        private List<AsteroidSprite> asteroids;
        private AsteroidParticleSystem particleSystem;
        private Song music;

        private Vector2 _shakeOffset;
        private float _shakeIntensity;
        private float _shakeDuration;

        private GameState _pendingState;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);
        }

        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            spriteBatch = ScreenManager.SpriteBatch;

            particleSystem = new AsteroidParticleSystem(ScreenManager.Game, 1000);
            ScreenManager.Game.Components.Add(particleSystem);

            LoadGameContent();

            if (asteroids == null)
            {
                asteroids = new List<AsteroidSprite>();
            }

            if (_pendingState != null)
            {
                LoadStateInternal(_pendingState);
                _pendingState = null;
            }
            else
            {
                InitializeAsteroids();
            }
        }

        private void LoadGameContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            background = content.Load<Texture2D>("Nebula");
            music = content.Load<Song>("Voxel Revolution");
            MediaPlayer.Play(music);
            MediaPlayer.IsRepeating = true;

            ship = new ShipSprite(ScreenManager.Game);
            ship.LoadContent(content);
        }

        private void InitializeAsteroids()
        {
            Random rand = new Random();
            asteroids = new List<AsteroidSprite>();

            for (int i = 0; i < 10; i++)
            {
                float randomAngularSpeed = (float)(rand.NextDouble());
                int textureWidth = 96;
                int textureHeight = 96;

                Vector2 randomPosition = new Vector2(
                    (float)rand.NextDouble() * (ScreenManager.GraphicsDevice.Viewport.Width - textureWidth),
                    (float)rand.NextDouble() * (ScreenManager.GraphicsDevice.Viewport.Height - textureHeight));

                var asteroid = new AsteroidSprite(randomPosition, randomAngularSpeed, particleSystem);
                asteroid.LoadContent(_content);
                asteroids.Add(asteroid);
            }
        }

        public override void Unload()
        {
            _content.Unload();
        }

        public void LoadState(GameState state)
        {
            //if (state == null) throw new ArgumentNullException(nameof(state));

            _pendingState = state;
        }

        private void LoadStateInternal(GameState state)
        {
            _activeTime = state.PlayTime;
            asteroids.Clear(); //asteroids = new List<AsteroidSprite>();

            foreach (var asteroidData in state.Asteroids)
            {
                if (asteroidData == null) continue;

                var asteroid = new AsteroidSprite(new Vector2(asteroidData.PositionX, asteroidData.PositionY), asteroidData.AngularVelocity, particleSystem);
                asteroid.LoadContent(_content);
                asteroid.LoadState(asteroidData);
                asteroids.Add(asteroid);
            }
        }

        public GameState SaveState()
        {
            var state = new GameState
            {
                Asteroids = new AsteroidData[asteroids.Count],
                PlayTime = _activeTime
            };

            for (int i = 0; i < asteroids.Count; i++)
            {
                if (asteroids[i] != null)
                {
                    state.Asteroids[i] = new AsteroidData();
                    asteroids[i].SaveState(state.Asteroids[i]);
                }
            }

            return state;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                _activeTime += gameTime.ElapsedGameTime;
                UpdateScreenShake(gameTime);
            }

            if (!IsActive)
            {
                return;
            }

            for (int i = 0; i < asteroids.Count; i++)
            {
                var asteroid = asteroids[i];
                if (asteroid != null && !asteroid.Destroyed)
                {
                    asteroid.Update(gameTime, ship);
                }
                else if (asteroid?.Destroyed == true)
                {
                    asteroids[i] = null;
                    StartScreenShake(3f, 0.15f);
                }
            }

            asteroids.RemoveAll(a => a == null);

            if (!asteroids.Any())
            {
                LoadingScreen.Load(ScreenManager, true, null, new VictoryScreen(_activeTime));
            }

            ship.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(this), ControllingPlayer);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphicsDevice = ScreenManager.GraphicsDevice;
            Matrix shakeTransform = Matrix.CreateTranslation(_shakeOffset.X, _shakeOffset.Y, 0);
            spriteBatch.Begin(transformMatrix: shakeTransform);
            spriteBatch.Draw(background, new Rectangle(0, 0, graphicsDevice.Viewport.Width + 20, graphicsDevice.Viewport.Height + 20), Color.White);
            ship.Draw(gameTime, spriteBatch);

            foreach (var asteroid in asteroids)
            {
                asteroid?.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
        }

        private void StartScreenShake(float intensity, float duration)
        {
            _shakeIntensity = intensity;
            _shakeDuration = duration;
        }

        private void UpdateScreenShake(GameTime gameTime)
        {
            if (_shakeDuration > 0)
            {
                _shakeDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                float currentIntensity = _shakeIntensity * (_shakeDuration / 0.2f);
                _shakeOffset = new Vector2(
                    (float)new Random().NextDouble() * 2 - 1,
                    (float)new Random().NextDouble() * 2 - 1
                ) * currentIntensity;
            }
            else
            {
                _shakeOffset = Vector2.Zero;
            }
        }
    }
}
