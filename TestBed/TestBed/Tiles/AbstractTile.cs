using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TestBed.Interface;

namespace TestBed.Tiles
{
    public abstract class AbstractTile : ITile
    {
        public AbstractTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle tileSize, float rotation)
        {
            SpriteBatch = spriteBatch;
            Texture = texture;
            Position = position;
            TileRectangle = tileSize;
            RotationAngle = rotation;
            UniqueID = Guid.NewGuid();
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
        /// Position of the tile.
        /// </summary>
        public virtual Vector2 Position { get; set; }

        /// <summary>
        /// Rotation of the tile.
        /// </summary>
        public float RotationAngle { get; private set; }

        /// <summary>
        /// Rectangle representing the tile.
        /// </summary>
        protected Rectangle TileRectangle { get; set; }

        /// <summary>
        /// Draws the tile.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Access to the SpirteBatch
        /// </summary>
        protected SpriteBatch SpriteBatch { get; private set; }

        public void Dispose() { }
    }
}
