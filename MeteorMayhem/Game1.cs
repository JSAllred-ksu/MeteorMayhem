﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameArchitectureExample.Screens;
using GameArchitectureExample.StateManagement;

namespace GameArchitectureExample
{
    // Sample showing how to manage different game states, with transitions
    // between menu screens, a loading screen, the game itself, and a pause
    // menu. This main game class is extremely simple: all the interesting
    // stuff happens in the ScreenManager component.
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private readonly ScreenManager _screenManager;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferredBackBufferWidth = 1280;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            var screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            Services.AddService(typeof(GameState), new GameState());

            _screenManager = new ScreenManager(this);
            Components.Add(_screenManager);

            AddInitialScreens();
        }

        private void AddInitialScreens()
        {
            _screenManager.AddScreen(new BackgroundScreen(), null);
            _screenManager.AddScreen(new MainMenuScreen(), null);
            //_screenManager.AddScreen(new SplashScreen(), null);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent() { }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                //Exit(); //commented out because why would i need to exit a game like that

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);
            base.Draw(gameTime);    // The real drawing happens inside the ScreenManager component
        }
    }
}
