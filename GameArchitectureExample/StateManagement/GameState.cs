using GameArchitectureExample.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameArchitectureExample.StateManagement
{
    [Serializable]
    public class GameState
    {
        // Asteroid states
        public AsteroidData[] Asteroids { get; set; }

        //public float PlayTime { get; set; }
    }

    [Serializable]
    public class AsteroidData
    {
        public Vector2 Position { get; set; }
        public float AngularVelocity { get; set; }
        public bool Destroyed { get; set; }
        public  AsteroidParticleSystem ParticleSystem;
    }
}
