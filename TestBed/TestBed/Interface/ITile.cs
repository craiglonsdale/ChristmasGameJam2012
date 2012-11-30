using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TestBed.Interface
{
    public interface ITile : IDisposable
    {
        /// <summary>
        /// The TypeID of this tile
        /// </summary>
        int TileID { get; set;  }

        /// <summary>
        /// Position of the tile.
        /// </summary>
        Vector2 Position { get; set;  }

        /// <summary>
        /// Rotation of the tile.
        /// </summary>
        float RotationAngle { get; }

        /// <summary>
        /// Draws the tile.
        /// </summary>
        void Draw();

        /// <summary>
        /// Unique ID to identify this tile by.
        /// </summary>
        Guid UniqueID { get; }
    }
}
