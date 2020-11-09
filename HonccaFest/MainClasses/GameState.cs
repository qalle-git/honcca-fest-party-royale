using HonccaFest.Files;
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
        public Vector2[,] Map;

        public GameState(string levelName)
        {
            Map = new Vector2[Globals.GameSize.X, Globals.GameSize.Y];

            List<int> levelTiles = FileHandler.GetFile(levelName);

            if (levelTiles.Count <= 0)
                return;

            for (int currentX = 0; currentX < Map.GetLength(0); currentX++)
                for (int currentY = 0; currentY < Map.GetLength(1); currentY++)
                {
                    int tileX = (currentX * 46) + (currentY * 2);
                    int tileY = (currentX * 46) + (currentY * 2) + 1;

                    Map[currentX, currentY] = new Vector2(levelTiles[tileX], levelTiles[tileY]);
                }
        }

        public abstract void Initialize(ref Player[] players);

        public abstract void Update(GameTime gameTime, Player[] players);

        public virtual void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            for (int currentX = 0; currentX < Map.GetLength(0); currentX++)
                for (int currentY = 0; currentY < Map.GetLength(1); currentY++)
                {
                    Vector2 drawTile = Map[currentX, currentY];

                    if (drawTile.Y > 0 || drawTile.X > 0)
                        spriteBatch.Draw(Main.TileSet, new Rectangle((int)currentX * Globals.TileSize.X, (int)currentY * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y), new Rectangle((int)drawTile.X * Globals.TileSize.X, (int)drawTile.Y * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y), Color.White);
                }
        }
    }
}
