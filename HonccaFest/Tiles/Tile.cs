using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.Tiles
{
    public struct Tile
    {
		public int TileX;
        public int TileY;

        public int TileIndex;
        public int TileLayer;   

        public Type TileType;

        public enum Type
        {
            NONE,
            COLLISION
        }
    }
}
