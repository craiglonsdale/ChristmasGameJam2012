using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace TestBed.Interface
{
    public interface ICollidableTile : ITile
    {
        /// <summary>
        /// Physics body for this collidable tile
        /// </summary>
        Body PhysicsBody { get; }

        void ApplyForce(Vector2 force);
    }
}
