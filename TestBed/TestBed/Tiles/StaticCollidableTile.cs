using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace TestBed.Tiles
{
    public class StaticCollidableTile : AbstractCollidableTile
    {
        public StaticCollidableTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle tileSize, float rotation, World physicsWorld, float density = 1.0f) 
            : base(spriteBatch, texture, position, tileSize, rotation, physicsWorld, density)
        { 
            PhysicsBody.BodyType = BodyType.Static;
        }

        public StaticCollidableTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Body physicsBody, int height, int width, float rotation, World physicsWorld)
            : base(spriteBatch, texture, position, physicsBody, new Rectangle(0, 0, width, height), rotation, physicsWorld)
        {
            PhysicsBody.BodyType = BodyType.Static;
        }
    }
}
