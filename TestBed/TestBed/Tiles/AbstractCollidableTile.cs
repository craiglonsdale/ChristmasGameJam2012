using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;
using TestBed.Interface;

namespace TestBed.Tiles
{
    public abstract class AbstractCollidableTile : ICollidableTile
    {
        World m_physicsWorld;

        public AbstractCollidableTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle tileSize, float rotation, World physicsWorld, float density) 
        {
            PhysicsBody = BodyFactory.CreateRectangle(physicsWorld,
                            ConvertUnits.ToSimUnits(tileSize.Width),
                            ConvertUnits.ToSimUnits(tileSize.Height),
                            density);

            Initialize(spriteBatch, texture, position, tileSize, rotation, physicsWorld);
        }

        public AbstractCollidableTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Body physicsBody, Rectangle tileSize, float rotation, World physicsWorld)
        {
            PhysicsBody = physicsBody;

            Initialize(spriteBatch, texture, position, tileSize, rotation, physicsWorld);
        }

        private void Initialize(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle tileSize, float rotation, World physicsWorld)
        {
            Texture = texture;
            TileRectangle = tileSize;
            SpriteBatch = spriteBatch;
            m_physicsWorld = physicsWorld;
            UniqueID = Guid.NewGuid();
            PhysicsBody.UserData = UniqueID;
            Position = ConvertUnits.ToSimUnits(position);
            RotationAngle = rotation;
        }

        /// <summary>
        /// The TypeID of this tile
        /// </summary>
        public int TileID { get; set; }

        /// <summary>
        /// Unique ID to identify this tile by.
        /// </summary>
        public Guid UniqueID { get; private set; }
        
        /// <summary>
        /// Texture for the tile.
        /// </summary>
        public virtual Texture2D Texture { get; private set; }

        /// <summary>
        /// Position of the tile in SimUnits.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return PhysicsBody.Position;
            }
            set
            {
                PhysicsBody.Position = value;
            }
        }

        /// <summary>
        /// Rotation of the tile.
        /// </summary>
        public float RotationAngle
        {
            get
            {
                return PhysicsBody.Rotation;
            }
            private set
            {
                PhysicsBody.Rotation = value;
            }
        }

        /// <summary>
        /// Rectangle representing the tile.
        /// </summary>
        protected Rectangle TileRectangle 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Draws the tile.
        /// </summary>
        public void Draw()
        {
            Vector2 renderPos = ConvertUnits.ToDisplayUnits(this.Position);
            Vector2 rect = new Vector2((float)(TileRectangle.Width / 2f), (float)TileRectangle.Height / 2f);
            
            SpriteBatch.Draw(this.Texture,
                             renderPos,
                             this.TileRectangle,
                             Color.White,
                             PhysicsBody.Rotation,
                             rect,
                             1.0f,
                             SpriteEffects.None,
                             0);
        }

        /// <summary>
        /// Access to the SpirteBatch
        /// </summary>
        protected SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Physics body for this collidable tile
        /// </summary>
        public Body PhysicsBody { get; protected set;}

        public void ApplyForce(Vector2 force)
        {
            PhysicsBody.ApplyLinearImpulse(force);
        }

        public void Dispose()
        {
            //m_physicsWorld.RemoveBody(PhysicsBody);
        }
    }
}
