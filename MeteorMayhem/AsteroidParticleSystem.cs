using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameArchitectureExample.Screens
{
    public class AsteroidParticleSystem : ParticleSystem
    {
        public AsteroidParticleSystem(Game game, int maxParticles) : base(game, maxParticles) { }

        protected override void InitializeConstants()
        {
            textureFilename = "particle";
            minNumParticles = 50;
            maxNumParticles = 100;
            blendState = BlendState.Additive;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            var velocity = RandomHelper.NextDirection() * RandomHelper.NextFloat(50, 200);
            var lifetime = RandomHelper.NextFloat(0.5f, 1.0f);
            var acceleration = -velocity / lifetime;
            var rotation = RandomHelper.NextFloat(0, MathHelper.TwoPi);
            var angularVelocity = RandomHelper.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
            var scale = RandomHelper.NextFloat(0.5f, 1f);
            var color = Color.Lerp(Color.Yellow, Color.Orange, RandomHelper.NextFloat(0, 1));

            p.Initialize(where, velocity, acceleration, color, lifetime, scale, rotation, angularVelocity);
        }

        protected override void UpdateParticle(ref Particle particle, float dt)
        {
            base.UpdateParticle(ref particle, dt);
            float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;
            particle.Scale = .1f + .25f * normalizedLifetime;
        }

        public void PlaceParticles(Vector2 where)
        {
            AddParticles(where + new Vector2(52, 52));//center
        }
    }
}