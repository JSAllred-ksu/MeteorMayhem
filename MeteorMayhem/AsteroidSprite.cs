using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CollisionExample.Collisions;
using Microsoft.Xna.Framework.Audio;
using GameArchitectureExample.StateManagement;

namespace GameArchitectureExample.Screens
{
    public class AsteroidSprite
    {
        private const float ANIMATION_SPEED = 0.1f;
        private const int FRAME_COUNT = 8;
        private const int FRAME_WIDTH = 96;

        private double animationTimer;
        private int animationFrame;

        private float angularVelocity;
        private float rotation;

        private SoundEffect rockBreak;

        private Vector2 position;

        private Texture2D texture;

        private BoundingCircle bounds;

        private Vector2 velocity;
        private float speed;
        private Rectangle screenBounds;
        private bool isMoving;

        private bool hasScreenBounds => screenBounds.Width > 0 && screenBounds.Height > 0;
        public bool Destroyed { get; set; } = false;

        private AsteroidParticleSystem _particleSystem;

        /// <summary>
        /// the bounding volume of the sprite
        /// </summary>
        public BoundingCircle Bounds => bounds;

        /// <summary>
        /// Creates a new asteroid sprite
        /// </summary>
        /// <param name="position">The position of the sprite in the game</param>
        public AsteroidSprite(Vector2 position, float angularVelocity, AsteroidParticleSystem particleSystem, bool shouldMove = false)
        {
            this.position = position;
            this.bounds = new BoundingCircle(position + new Vector2(52, 52), 30);
            this.angularVelocity = angularVelocity;
            this._particleSystem = particleSystem;
            this.isMoving = shouldMove;

            if (shouldMove)
            {
                Random rand = new Random();
                speed = rand.Next(10, 100);

                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                velocity = new Vector2(
                    (float)Math.Cos(angle),
                    (float)Math.Sin(angle)
                ) * speed;
            }
        }

        public void SetScreenBounds(Rectangle bounds)
        {
            screenBounds = bounds;
        }
        private void EmitDestructionParticles()
        {
            if (_particleSystem != null)
            {
                _particleSystem.PlaceParticles(position);
            }
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Asteroid");
            rockBreak = content.Load<SoundEffect>("rockbreak");
        }

        public void SaveState(AsteroidData data)
        {
            data.PositionX = position.X;
            data.PositionY = position.Y;
            data.AngularVelocity = angularVelocity;
            data.Destroyed = Destroyed;
            data.VelocityX = velocity.X;
            data.VelocityY = velocity.Y;
            data.IsMoving = isMoving;

            if (hasScreenBounds)
            {
                data.ScreenWidth = screenBounds.Width;
                data.ScreenHeight = screenBounds.Height;
            }
        }

        public void LoadState(AsteroidData data)
        {
            position = new Vector2(data.PositionX, data.PositionY);
            angularVelocity = data.AngularVelocity;
            Destroyed = data.Destroyed;
            velocity = new Vector2(data.VelocityX, data.VelocityY);
            isMoving = data.IsMoving;
            bounds = new BoundingCircle(position + new Vector2(52, 52), 30);

            if (data.ScreenWidth > 0 && data.ScreenHeight > 0)
            {
                screenBounds = new Rectangle(0, 0, data.ScreenWidth, data.ScreenHeight);
            }
        }

        public void Update(GameTime gameTime, ShipSprite ship)
        {
            if (Destroyed) return;

            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            rotation += angularVelocity / 3;

            if (isMoving)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 newPosition = position + (velocity * deltaTime);

                if (newPosition.X < -FRAME_WIDTH) newPosition.X = screenBounds.Width;
                if (newPosition.X > screenBounds.Width) newPosition.X = -FRAME_WIDTH;
                if (newPosition.Y < -FRAME_WIDTH) newPosition.Y = screenBounds.Height;
                if (newPosition.Y > screenBounds.Height) newPosition.Y = -FRAME_WIDTH;

                position = newPosition;
                bounds.Center = position + new Vector2(52, 52);
            }

            if (bounds.CollidesWith(ship.Bounds))
            {
                if (animationTimer > ANIMATION_SPEED)
                {
                    animationFrame++;
                    if (animationFrame >= FRAME_COUNT)
                    {
                        rockBreak.Play();
                        Destroyed = true;
                        EmitDestructionParticles();
                        return;
                    }
                    animationTimer -= ANIMATION_SPEED;
                }
            }
        }

        /// <summary>
        /// Draws the animated sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Destroyed) return;

            Rectangle source = new Rectangle(animationFrame * FRAME_WIDTH, 0, FRAME_WIDTH, texture.Height);
            Vector2 origin = new Vector2(FRAME_WIDTH / 2, texture.Height / 2);

            spriteBatch.Draw(texture, position + origin, source, Color.Lerp(Color.White, Color.Red, 0.2f), rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
