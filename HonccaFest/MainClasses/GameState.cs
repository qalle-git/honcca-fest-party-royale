using HonccaFest.Files;
using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.MainClasses
{
    public abstract class GameState
    {
        public Tile[,][] Map;

        public string LevelName;

        private const int maxLayersPerTile = 10;

        public GameState(string levelName)
        {
            LevelName = levelName;

            Map = new Tile[Globals.GameSize.X, Globals.GameSize.Y][];

			for (int currentX = 0; currentX < Map.GetLength(0); currentX++)
			{
				for (int currentY = 0; currentY < Map.GetLength(1); currentY++)
				{
                    Map[currentX, currentY] = new Tile[maxLayersPerTile];
				}
			}

            List<int> levelTiles = FileHandler.GetFile(levelName);

            if (levelTiles.Count <= 0)
                return;

			for (int currentLineIndex = 0; currentLineIndex < levelTiles.Count; currentLineIndex += 5)
			{
                int tileX = levelTiles[currentLineIndex];
                int tileY = levelTiles[currentLineIndex + 1];

                int tileIndex = levelTiles[currentLineIndex + 2];

                int tileCollision = levelTiles[currentLineIndex + 3];

                int tileLayer = levelTiles[currentLineIndex + 4];

				Tile newTile = new Tile()
				{
                    TileX = tileX,
                    TileY = tileY,
					TileIndex = tileIndex,
					TileType = (Tile.Type)tileCollision,
                    TileLayer = tileLayer
				};

                Map[tileX, tileY][tileLayer] = newTile;
			}
        }

        public abstract void Initialize(ref Player[] players);

        public abstract void Update(GameTime gameTime, Player[] players);

        public virtual void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            int numTilesX = Main.TileSet.Width / Globals.TileSize.X;

            for (int currentX = 0; currentX < Map.GetLength(0); currentX++)
            {
                for (int currentY = 0; currentY < Map.GetLength(1); currentY++)
                {
					for (int currentTileIndex = 0; currentTileIndex < Map[currentX, currentY].Length; currentTileIndex++)
					{
                        Tile drawTile = Map[currentX, currentY][currentTileIndex];

                        if (drawTile.TileIndex > 0)
                                spriteBatch.Draw(Main.TileSet, new Rectangle((int)currentX * Globals.TileSize.X, (int)currentY * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y), new Rectangle(drawTile.TileIndex % numTilesX * Globals.TileSize.X, drawTile.TileIndex / numTilesX * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y), Color.White);
					}
                }
            }
        }
    }
}
