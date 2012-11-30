using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestBed.Interface;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TestBed.Tiles
{
    public class Tile : AbstractTile
    {
        public Tile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle tileSize, float rotation) 
            : base(spriteBatch, texture, position, tileSize, rotation)
        {
            
        }

        public override void Draw()
        {
            SpriteBatch.Draw(this.Texture,
                             this.Position,
                             this.TileRectangle,
                             Color.White,
                             this.RotationAngle,
                             new Vector2(TileRectangle.Width / 2f, TileRectangle.Height / 2f),
                             1.0f,
                             SpriteEffects.None,
                             0);
        }
    }
}
