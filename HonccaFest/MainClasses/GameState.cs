﻿// GameState.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using HonccaFest.Files;
using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace HonccaFest.MainClasses
{
    public abstract class GameState
    {
        public Tile[,][] Map;

        public string LevelName;

        public TimeSpan StartedGameState = TimeSpan.Zero;

        private const int maxLayersPerTile = 10;

        public GameState(string levelName)
        {
            LevelName = levelName;

            Map = new Tile[Globals.GameSize.X, Globals.GameSize.Y][];

            // Create a default tile on each x and y coordinate.
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

            // Create the map with the saved tiles.
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

        /// <summary>
        /// Draw the map you've chosen in the constructor.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch from Main.</param>
        /// <param name="players">The player array filled with all the player objects.</param>
        public virtual void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            int numTilesX = Main.GraphicsHandler.GetSprite("TileSheet").Width / Globals.TileSize.X;

            for (int currentX = 0; currentX < Map.GetLength(0); currentX++)
            {
                for (int currentY = 0; currentY < Map.GetLength(1); currentY++)
                {
					for (int currentTileIndex = 0; currentTileIndex < Map[currentX, currentY].Length; currentTileIndex++)
					{
                        Tile drawTile = Map[currentX, currentY][currentTileIndex];

                        if (drawTile.TileIndex > 0)
                            spriteBatch.Draw(Main.GraphicsHandler.GetSprite("TileSheet"), new Rectangle(currentX * Globals.TileSize.X, currentY * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y), new Rectangle(drawTile.TileIndex % numTilesX * Globals.TileSize.X, drawTile.TileIndex / numTilesX * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y), Color.White);
					}
                }
            }
        }

        /// <summary>
        /// This method makes the gamemode accept stuns from players. They can stun eachother with Yellow button.
        /// </summary>
        /// <param name="players">The player array filled with all the player objects.</param>
        public virtual void StunHandler(GameTime gameTime, Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                for (int currentColliderIndex = 0; currentColliderIndex < players.Length; currentColliderIndex++)
                    if (currentPlayerIndex != currentColliderIndex)
                    {
                        Player colliderPlayer = players[currentColliderIndex];

                        // Checks if the player is active and close to another player. If Yellow key is pressed then stun that duck.
                        if (currentPlayer.Active && Vector2.Distance(currentPlayer.CurrentPixelPosition, colliderPlayer.CurrentPixelPosition) <= Globals.TileSize.X && currentPlayer.JustPressedActionKey(ArcadeButton.Yellow))
                        {
                            Globals.DebugPrint($"{currentPlayerIndex} just stunned {currentColliderIndex} {Vector2.Distance(currentPlayer.CurrentPixelPosition, currentPlayer.CurrentPixelPosition)}.");

                            colliderPlayer.GetStunned(currentPlayer);
                        }
                    }
            }
        }
    }
}
