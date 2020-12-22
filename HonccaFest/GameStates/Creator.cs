// Creator.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using HonccaFest.Files;
using HonccaFest.MainClasses;
using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.GameStates
{
    class Creator : GameState
    {
        private int currentTileIndex;
        private int collisionEnabled;

        private Vector2 currentPosition;
        private Vector2 currentPixelPosition;

        private const string mapName = "DuckOut";

        public Creator() : base(mapName)
        {

        }

        public override void Initialize(ref Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].ForceMove(new Vector2(-1, -1));
        }

        public override void Update(GameTime gameTime, Player[] players)
        {
            HandleInput(gameTime);

            HandleMovement(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            if (gameTime.TotalGameTime > lastMovement + movementCooldown)
            {
                KeyboardState keyboardState = Keyboard.GetState();

                bool updateMovement = false;

                foreach (Keys key in keyboardState.GetPressedKeys())
                {
                    switch (key)
                    {
                        case Keys.Enter:
                            AddToMap();

                            updateMovement = true;

                            break;
                        case Keys.Escape:
                            break;
                        case Keys.Space:
                            SaveMap();

                            updateMovement = true;

                            break;
                        case Keys.Left:
                            if (currentTileIndex > 0)
                                currentTileIndex--;

                            updateMovement = true;

                            break;
                        case Keys.Up:
                            if (currentTileIndex > 0)
                                currentTileIndex -= 14;

                            updateMovement = true;

                            break;
                        case Keys.Right:
                            currentTileIndex++;

                            updateMovement = true;

                            break;
                        case Keys.Down:
                            currentTileIndex += 14;

                            updateMovement = true;

                            break;
                        case Keys.W:
                            currentPosition.Y--;

                            updateMovement = true;

                            break;
                        case Keys.A:
                            currentPosition.X--;

                            updateMovement = true;

                            break;
                        case Keys.S:
                            currentPosition.Y++;

                            updateMovement = true;

                            break;
                        case Keys.D:
                            currentPosition.X++;

                            updateMovement = true;

                            break;
                        case Keys.G:
                            collisionEnabled = (int)Tile.Type.COLLISION == collisionEnabled ? 0 : 1;

                            updateMovement = true;

                            break;
                        case Keys.Back:
                            Main.Instance.ChangeGameState(new MainMenu());

                            break;
                        case Keys.Delete:
                            Map = new Tile[Globals.GameSize.X, Globals.GameSize.Y][];

                            for (int currentX = 0; currentX < Map.GetLength(0); currentX++)
                            {
                                for (int currentY = 0; currentY < Map.GetLength(1); currentY++)
                                {
                                    Map[currentX, currentY] = new Tile[3];
                                }
                            }

                            break;
                        case Keys.E:
                            RemoveTopLayer();

                            updateMovement = true;

                            break;
                        case Keys.K:
                            FillMap();

                            updateMovement = true;

                            break;
                        default:
                            break;
                    }
                }

                if (updateMovement)
                    lastMovement = gameTime.TotalGameTime;
            }
        }

		private void FillMap()
		{
            for (int currentX = 0; currentX < Map.GetLength(0); currentX++)
                for (int currentY = 0; currentY < Map.GetLength(1); currentY++)
                {
                    Tile[] tiles = Map[currentX, currentY];

                    tiles[0] = new Tile()
                    {
                        TileX = (int)currentX,
                        TileY = (int)currentY,
                        TileIndex = currentTileIndex,
                        TileType = Tile.Type.NONE,
                        TileLayer = 0
                    };
                }

        }

        private void SaveMap()
        {
            List<int> saveList = new List<int>();

			for (int currentX = 0; currentX < Map.GetLength(0); currentX++)
				for (int currentY = 0; currentY < Map.GetLength(1); currentY++)
				{
                    Tile[] tiles = Map[currentX, currentY];

					for (int currentTileIndex = 0; currentTileIndex < tiles.Length; currentTileIndex++)
					{
                        if (tiles[currentTileIndex].TileIndex > 0)
						{
                            saveList.Add((int)tiles[currentTileIndex].TileX);
                            saveList.Add((int)tiles[currentTileIndex].TileY);
                            saveList.Add((int)tiles[currentTileIndex].TileIndex);
                            saveList.Add((int)tiles[currentTileIndex].TileType);
                            saveList.Add((int)tiles[currentTileIndex].TileLayer);
                        }
					}
				}

			FileHandler.AddFile(mapName, saveList);

            Console.WriteLine($"Saving Map: {mapName}");
        }

        private void AddToMap()
        {
            if (currentPosition.X < 0)
                return;
            else if (currentPosition.X > Globals.GameSize.X - 1)
                return;
            else if (currentPosition.Y < 0)
                return;
            else if (currentPosition.Y > Globals.GameSize.Y - 1)
                return;

            Tile[] tiles = Map[(int)currentPosition.X, (int)currentPosition.Y];

            int freeLayer = -1;

			for (int currentTileIndex = 0; currentTileIndex < tiles.Length; currentTileIndex++)
			{
                if (tiles[currentTileIndex].TileIndex == 0)
				{
                    freeLayer = currentTileIndex;

                    break;
				}
			}

            if (freeLayer == -1)
                return;

            Tile newTile = new Tile()
            {
                TileX = (int)currentPosition.X,
                TileY = (int)currentPosition.Y,
                TileIndex = currentTileIndex,
                TileType = (Tile.Type)collisionEnabled,
                TileLayer = freeLayer
            };

			tiles[freeLayer] = newTile;
		}

        private void RemoveTopLayer()
        {
            if (currentPosition.X < 0)
                return;
            else if (currentPosition.X > Globals.GameSize.X - 1)
                return;
            else if (currentPosition.Y < 0)
                return;
            else if (currentPosition.Y > Globals.GameSize.Y - 1)
                return;

            Tile[] tiles = Map[(int)currentPosition.X, (int)currentPosition.Y];

            for (int currentTileIndex = tiles.Length - 1; currentTileIndex >= 0; currentTileIndex--)
            {
                Console.WriteLine(tiles[currentTileIndex].TileIndex);

                if (tiles[currentTileIndex].TileIndex > 0)
                {
                    Console.WriteLine("Removing tile?");

                    Map[(int)currentPosition.X, (int)currentPosition.Y][currentTileIndex] = new Tile();

                    break;
                }
            }
        }

        private TimeSpan movementCooldown = TimeSpan.FromMilliseconds(150);
        private TimeSpan lastMovement = TimeSpan.Zero;

        private void HandleMovement(GameTime gameTime)
        {
            Vector2 newPosition = new Vector2(currentPosition.X * Globals.TileSize.X, currentPosition.Y * Globals.TileSize.Y);

            if (currentPixelPosition != newPosition)
                currentPixelPosition = new Vector2(currentPosition.X * Globals.TileSize.X, currentPosition.Y * Globals.TileSize.Y);
        }

		public override void Draw(SpriteBatch spriteBatch, Player[] players)
		{
            base.Draw(spriteBatch, players);

            int numTilesX = Main.GraphicsHandler.GetSprite("TileSheet").Width / Globals.TileSize.X;

            Rectangle tileRectangle = new Rectangle(currentTileIndex % numTilesX * Globals.TileSize.X, currentTileIndex / numTilesX * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y);

            if (currentTileIndex > 0)
                spriteBatch.Draw(Main.GraphicsHandler.GetSprite("TileSheet"), new Rectangle((int)currentPixelPosition.X, (int)currentPixelPosition.Y, Globals.TileSize.X, Globals.TileSize.Y), tileRectangle, Color.White);

            spriteBatch.Draw(Main.GraphicsHandler.GetSprite("OutlineRectangle"), new Rectangle((int)currentPixelPosition.X, (int)currentPixelPosition.Y, Globals.TileSize.X, Globals.TileSize.Y), Color.White);

            spriteBatch.DrawString(Main.DebugFont, $"X: {currentPosition.X}\nY: {currentPosition.Y}", new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(Main.DebugFont, $"Tile: {currentTileIndex}", new Vector2(0, 40), Color.White);
            spriteBatch.DrawString(Main.DebugFont, $"Collision: {(collisionEnabled == (int)Tile.Type.COLLISION ? "ACTIVATED" : "DEACTIVATED")} - G", new Vector2(0, 80), Color.White);
            spriteBatch.DrawString(Main.DebugFont, $"Erase Top Layer - E", new Vector2(0, 100), Color.White);
            spriteBatch.DrawString(Main.DebugFont, $"ENTER - PLACE TILE\nSPACE - SAVE MAP {mapName}", new Vector2(0, 120), Color.White);
            spriteBatch.DrawString(Main.DebugFont, $"BACKSPACE - GO BACK", new Vector2(0, 180), Color.White);
        }
    }
}
