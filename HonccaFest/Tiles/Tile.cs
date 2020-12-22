// Tile.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

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
