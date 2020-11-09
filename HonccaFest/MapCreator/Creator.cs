using HonccaFest.Files;
using HonccaFest.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace HonccaFest.MapCreator
{
    class Creator
    {
        private Vector2[,] currentMap;

        private Vector2 currentTile;
        private Vector2 currentPosition;
        private Vector2 currentPixelPosition;

        private const string mapName = "Quackory";

        public Creator()
        {
            currentMap = new Vector2[Globals.GameSize.X, Globals.GameSize.Y];

            LoadMap(mapName);
        }

        public void Update(GameTime gameTime)
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
                            if (currentTile.X > 0)
                                currentTile.X--;

                            updateMovement = true;

                            break;
                        case Keys.Up:
                            if (currentTile.Y > 0)
                                currentTile.Y--;

                            updateMovement = true;

                            break;
                        case Keys.Right:
                            currentTile.X++;

                            updateMovement = true;

                            break;
                        case Keys.Down:
                            currentTile.Y++;

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
                        default:
                            break;
                    }
                }

                if (updateMovement)
                    lastMovement = gameTime.TotalGameTime;
            }
        }
        
        private void LoadMap(string mapName)
        {
            List<int> levelTiles = FileHandler.GetFile(mapName);

            if (levelTiles.Count <= 0)
                return;

            for (int currentX = 0; currentX < currentMap.GetLength(0); currentX++)
                for (int currentY = 0; currentY < currentMap.GetLength(1); currentY++)
                {
                    int tileX = (currentX * 46) + (currentY * 2);
                    int tileY = (currentX * 46) + (currentY * 2) + 1;

                    currentMap[currentX, currentY] = new Vector2(levelTiles[tileX], levelTiles[tileY]);
                }
        }

        private void SaveMap()
        {
            List<int> saveList = new List<int>();

            for (int currentX = 0; currentX < currentMap.GetLength(0); currentX++)
                for (int currentY = 0; currentY < currentMap.GetLength(1); currentY++)
                {
                    saveList.Add((int)currentMap[currentX, currentY].X);
                    saveList.Add((int)currentMap[currentX, currentY].Y);
                }

            FileHandler.AddFile(mapName, saveList);

            Console.WriteLine($"Saving Map: {mapName}");
        }

        private void AddToMap()
        {
            currentMap[(int)currentPosition.X, (int)currentPosition.Y] = currentTile;
        }

        private TimeSpan movementCooldown = TimeSpan.FromMilliseconds(150);
        private TimeSpan lastMovement = TimeSpan.Zero;

        private void HandleMovement(GameTime gameTime)
        {
            Vector2 newPosition = new Vector2(currentPosition.X * Globals.TileSize.X, currentPosition.Y * Globals.TileSize.Y);

            if (currentPixelPosition != newPosition)
                currentPixelPosition = new Vector2(currentPosition.X * Globals.TileSize.X, currentPosition.Y * Globals.TileSize.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle tileRectangle = new Rectangle((int)currentTile.X * Globals.TileSize.X, (int)currentTile.Y * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y);

            for (int currentX = 0; currentX < currentMap.GetLength(0); currentX++)
            {
                for (int currentY = 0; currentY < currentMap.GetLength(1); currentY++)
                {
                    Vector2 drawTile = currentMap[currentX, currentY];

                    if (drawTile.Y > 0 || drawTile.X > 0)
                        spriteBatch.Draw(Main.TileSet, new Rectangle((int)currentX * Globals.TileSize.X, (int)currentY * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y), new Rectangle((int)drawTile.X * Globals.TileSize.X, (int)drawTile.Y * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y), Color.White);
                }
            }

            if (currentTile.Y > 0 || currentTile.X > 0)
                spriteBatch.Draw(Main.TileSet, new Rectangle((int)currentPixelPosition.X, (int)currentPixelPosition.Y, Globals.TileSize.X, Globals.TileSize.Y), tileRectangle, Color.White);

            spriteBatch.Draw(Main.OutlineRectangle, new Rectangle((int)currentPixelPosition.X, (int)currentPixelPosition.Y, Globals.TileSize.X, Globals.TileSize.Y), Color.White);

            spriteBatch.DrawString(Main.DebugFont, $"X: {currentPosition.X}\nY: {currentPosition.Y}", new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(Main.DebugFont, $"TileX: {currentTile.X}\nTileY: {currentTile.Y}", new Vector2(0, 40), Color.White);
            spriteBatch.DrawString(Main.DebugFont, $"ENTER - PLACERA\nSPACE - SPARA", new Vector2(0, 80), Color.White);
        }
    }
}
