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
        public AsteroidData[] Asteroids { get; set; }
        public TimeSpan PlayTime { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public Color ShipColor { get; set; }
        public GameplayScreen.GameMode GameMode { get; set; }
        public int CurrentLevel { get; set; }
        public int TotalAsteroidsDestroyed { get; set; }
    }

    [Serializable]
    public class AsteroidData
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float AngularVelocity { get; set; }
        public bool Destroyed { get; set; }
    }
}
