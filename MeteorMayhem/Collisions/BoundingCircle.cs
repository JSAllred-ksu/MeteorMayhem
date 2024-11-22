using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CollisionExample.Collisions
{
    /// <summary>
    /// a struct representing circular bounds
    /// </summary>
    public struct BoundingCircle
    {
        /// <summary>
        /// the center of the BoundingCircle
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 Center;

        /// <summary>
        /// the radius of the BoundingCircle
        /// </summary>
        public float Radius;

        /// <summary>
        /// constructs a new BoundingCircle
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public BoundingCircle(Microsoft.Xna.Framework.Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// tests for a collision between this BoundingCircle and another
        /// </summary>
        /// <param name="other">the other BoundingCircle</param>
        /// <returns>true for collision, else false</returns>
        public bool CollidesWith(BoundingCircle other)
        {
            return CollisionHelper.Collides(this, other);
        }

        public bool CollidesWith(BoundingRectangle other)
        {
            return CollisionHelper.Collides(this, other);
        }
    }
}
