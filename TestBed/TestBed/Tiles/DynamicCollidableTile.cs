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
        private Vector3 m_3dPosition;

        public DynamicCollidableTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle tileSize, float rotation, World physicsWorld, float density = 1.0f)
            : base(spriteBatch, texture, position, tileSize, rotation, physicsWorld, density)
        {
            PhysicsBody.BodyType = BodyType.Dynamic;
            PhysicsBody.Friction = 0.1f;

            m_3dPosition = new Vector3();
        }

        public Vector3 PositionIn3DDisplay
        {
            get
            {
                m_3dPosition.X = ConvertUnits.ToDisplayUnits(PhysicsBody.Position.X);
                m_3dPosition.Y = ConvertUnits.ToDisplayUnits(PhysicsBody.Position.Y);
                m_3dPosition.Z = 0;
                return m_3dPosition;
            }
        }

        //public DynamicCollidableTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Body physicsBody, float height, float width, float rotation)
        //    : base(spriteBatch, texture, position, tileSize, rotation, physicsWorld, density)
        //{
        //    PhysicsBody.BodyType = BodyType.Dynamic;
        //    PhysicsBody.Friction = 0.1f;
        //}

    }
}
