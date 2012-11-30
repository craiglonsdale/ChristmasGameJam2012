using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace TestBed.Tiles
{
    public class DynamicCollidableTile : AbstractCollidableTile
    {
        public DynamicCollidableTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle tileSize, float rotation, World physicsWorld, float density = 1.0f)
            : base(spriteBatch, texture, position, tileSize, rotation, physicsWorld, density)
        {
            PhysicsBody.BodyType = BodyType.Dynamic;
            PhysicsBody.Friction = 0.1f;
        }

        //public DynamicCollidableTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Body physicsBody, float height, float width, float rotation)
        //    : base(spriteBatch, texture, position, tileSize, rotation, physicsWorld, density)
        //{
        //    PhysicsBody.BodyType = BodyType.Dynamic;
        //    PhysicsBody.Friction = 0.1f;
        //}

    }
}
