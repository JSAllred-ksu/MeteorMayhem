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
        private Color shipColor = Color.White;
        public Color ShipColor
        {
            get => shipColor;
            set => shipColor = value;
        }
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
