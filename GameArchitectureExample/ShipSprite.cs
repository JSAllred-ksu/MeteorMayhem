using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using CollisionExample.Collisions;
using GameArchitectureExample.StateManagement;

namespace GameArchitectureExample.Screens
{
    /// <summary>
    /// A class representing the players' ship
    /// </summary>
    public class ShipSprite
    {
        const float LINEAR_ACCELERATION = 150;
        const float ANGULAR_ACCELERATION = 4;
        const int FRAME_WIDTH = 20;
        const int FRAME_HEIGHT = 32;
        const int NUM_FRAMES = 7;
        const double TIME_PER_FRAME = 0.1;

        int animationFrame = 0;
        double animationTimer;

        Game game;
        Texture2D texture;
        BoundingCircle bounds;
        public BoundingCircle Bounds => bounds;

        Vector2 position;
        Vector2 velocity;
        Vector2 direction;

        float angle;
        float angularVelocity;

        private Color shipColor = Color.White;
        public Color ShipColor
        {
            get => shipColor;
            set => shipColor = value;
        }

        /// <summary>
        /// Creates the ship sprite
        /// </summary>
        public ShipSprite(Game game)
        {
            this.game = game;
            this.position = new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2); //spawn
            this.direction = -Vector2.UnitY;
            this.bounds = new BoundingCircle(position + new Vector2(FRAME_WIDTH / 2, FRAME_HEIGHT / 2), FRAME_HEIGHT / 2);
        }

        /// <summary>
        /// Loads the sprite texture
        /// </summary>
        /// <param name="content">The content manager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("RocketDrill");
        }

        public void LoadColor(Game game)
        {
            var gameState = game.Services.GetService<GameState>();
            if (gameState != null)
            {
                shipColor = gameState.ShipColor;
            }
        }

        //public void SaveState(GameState state)
        //{
        //    state.ShipPosition = position;
        //    state.ShipVelocity = velocity;
        //    state.ShipAngle = angle;
        //    state.ShipAngularVelocity = angularVelocity;
        //}

        //public void LoadState(GameState state)
        //{
        //    position = state.ShipPosition;
        //    velocity = state.ShipVelocity;
        //    angle = state.ShipAngle;
        //    angularVelocity = state.ShipAngularVelocity;

        //    // Update direction based on loaded angle
        //    direction.X = (float)Math.Sin(angle);
        //    direction.Y = (float)-Math.Cos(angle);

        //    // Update bounds
        //    bounds = new BoundingCircle(position + new Vector2(FRAME_WIDTH / 2, FRAME_HEIGHT / 2), FRAME_HEIGHT / 2);
        //}

        /// <summary>
        /// Updates the ship sprite
        /// </summary>
        /// <param name="gameTime">An object representing time in the game</param>
        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // reset acceleration to zero for each frame
            Vector2 acceleration = new Vector2(0, 0);

            // check for rotation
            if (keyboardState.IsKeyDown(Keys.A))
            {
                angularVelocity -= ANGULAR_ACCELERATION * t;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                angularVelocity += ANGULAR_ACCELERATION * t;
            }

            // update rotation
            angle += angularVelocity * t;
            direction.X = (float)Math.Sin(angle);
            direction.Y = (float)-Math.Cos(angle);

            // update bounds
            bounds = new BoundingCircle(position + new Vector2(FRAME_WIDTH / 2, FRAME_HEIGHT / 2), FRAME_HEIGHT / 2);

            // thruster 
            if (keyboardState.IsKeyDown(Keys.W))
            {
                acceleration += direction * LINEAR_ACCELERATION;
            }

            // apply acceleration, update velocity and position
            velocity += acceleration * t;
            position += velocity * t;

            // wrap the ship to keep it on screen 
            var viewport = game.GraphicsDevice.Viewport;
            if (position.Y < 0) position.Y = viewport.Height;
            if (position.Y > viewport.Height) position.Y = 0;
            if (position.X < 0) position.X = viewport.Width;
            if (position.X > viewport.Width) position.X = 0;
        }

        /// <summary>
        /// Draws the sprite
        /// </summary>
        /// <param name="gameTime">An object representing time in the game</param>
        /// <param name="spriteBatch">The SpriteBatch to draw with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > TIME_PER_FRAME)
            {
                animationFrame++;
                if (animationFrame >= NUM_FRAMES) animationFrame = 0;
                animationTimer -= TIME_PER_FRAME;
            }

            var source = new Rectangle((texture.Width - FRAME_WIDTH) / 2, animationFrame * FRAME_HEIGHT, FRAME_WIDTH, FRAME_HEIGHT);
            Vector2 origin = new Vector2(FRAME_WIDTH / 2f, FRAME_HEIGHT / 2f);

            spriteBatch.Draw(texture, position, source, shipColor, angle, origin, 2f, SpriteEffects.None, 0f);
        }
    }
}
