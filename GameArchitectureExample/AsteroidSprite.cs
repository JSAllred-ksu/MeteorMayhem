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

        public bool Destroyed { get; set; } = false;

        private AsteroidParticleSystem _particleSystem;

        /// <summary>
        /// the bounding volume of the sprite
        /// </summary>
        public BoundingCircle Bounds => bounds;

        /// <summary>
        /// Creates a new coin sprite
        /// </summary>
        /// <param name="position">The position of the sprite in the game</param>
        public AsteroidSprite(Vector2 position, float angularVelocity, AsteroidParticleSystem particleSystem)
        {
            this.position = position;
            this.bounds = new BoundingCircle(position + new Vector2(52, 52), 30);
            this.angularVelocity = angularVelocity;
            this._particleSystem = particleSystem;
        }
        private void EmitDestructionParticles()
        {
            _particleSystem.PlaceParticles(position);
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
            data.Position = position;
            data.AngularVelocity = angularVelocity;
            data.Destroyed = Destroyed;
            data.ParticleSystem = _particleSystem;
        }

        public void LoadState(AsteroidData data)
        {
            position = data.Position;
            angularVelocity = data.AngularVelocity;
            Destroyed = data.Destroyed;
            _particleSystem = data.ParticleSystem;
            bounds = new BoundingCircle(position + new Vector2(52, 52), 30);
        }

        public void Update(GameTime gameTime, ShipSprite ship)
        {
            if (Destroyed) return;

            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            rotation += angularVelocity / 3;

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
