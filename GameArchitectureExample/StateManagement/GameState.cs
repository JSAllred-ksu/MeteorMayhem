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
        // Ship state
        public Vector2 ShipPosition { get; set; }
        public Vector2 ShipVelocity { get; set; }
        public float ShipAngle { get; set; }
        public float ShipAngularVelocity { get; set; }

        // Asteroid states
        public AsteroidData[] Asteroids { get; set; }

        // Score/game progress
        public int Score { get; set; }
        public int Level { get; set; }

        // Additional game stats if needed
        public float PlayTime { get; set; }
    }

    [Serializable]
    public class AsteroidData
    {
        public Vector2 Position { get; set; }
        public float AngularVelocity { get; set; }
        public bool Destroyed { get; set; }
        public int AnimationFrame { get; set; }
    }
}
