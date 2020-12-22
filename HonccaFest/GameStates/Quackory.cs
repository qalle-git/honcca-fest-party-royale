// Quackory.cs
// Author Omid Jawadi
// LBS Kreativa Gymnasiet

using HonccaFest.Files;
using HonccaFest.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace HonccaFest.GameStates
{
    class Quackory : GameState
    {
        private Enum safeTile;

        private bool roundOver = false;

        private readonly Vector2[] spawnPoints = new Vector2[4];

        private Point platformSize;
        private Point platformOffset;
        private Point safeTileOffset;

        private int[] timesLived = new int[4];

        List<Player> ExecutionOrder = new List<Player>(); // List sorting the player deaths in order

        // Enum where each element represents a different type of tile in the tilesheet
        public enum TileTypes
        {
            BANANA = 9,
            ORANGE = 10,
            PICKLE = 11,
            TOMATO = 12,
        }

        // Enum where each element represents a different stage in the gamemode
        public enum Stage
        {
            START,
            RANDOMIZE,
            VISUALIZE,
            POSITION,
            EXECUTE
        }

        public Stage CurrentStage = Stage.START;

        // How long each stage will last in seconds
        private readonly double[] stageTimeSpan = new double[]
        {
            2, 2, 9, 6, 3
        };

        public Quackory() : base("Quackory")
        {
        }

        public override void Initialize(ref Player[] players)
        {
            platformSize = new Point(10, 10);
            platformOffset = new Point((Globals.GameSize.X - platformSize.X) / 2, (Globals.GameSize.Y - platformSize.Y) / 2);
            safeTileOffset = new Point((platformOffset.X - 5), platformOffset.Y + 5);

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player player = players[currentPlayerIndex];
                player.Active = true;

                spawnPoints[currentPlayerIndex] = new Vector2(currentPlayerIndex + platformOffset.X, 2 + platformOffset.Y);
                player.ForceMove(spawnPoints[currentPlayerIndex]);
            }

            Main.MusicHandler.Play("wandering_maze");

            TileVisible(false, false, false);
            PlatformWalls();
        }

        public override void Update(GameTime gameTime, Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].Update(gameTime, Map);

            StageHandler(gameTime, players);

            EndGame(gameTime, players);

            Console.WriteLine(CurrentStage);
        }

        // Sets collision tiles outside the platform so the players can't walk outside it
        public void PlatformWalls()
        {
            for (int currentX = 0; currentX < platformSize.X; currentX++)
            {
                Map[currentX + platformOffset.X, platformSize.Y + platformOffset.Y][0].TileType = Tiles.Tile.Type.COLLISION;
                Map[currentX + platformOffset.X, platformOffset.Y - 1][0].TileType = Tiles.Tile.Type.COLLISION;

                Map[currentX + platformOffset.X, platformOffset.Y - 1][0].TileIndex = 3;
                Map[currentX + platformOffset.X, platformSize.Y + platformOffset.Y][0].TileIndex = 3;
            }

            for (int currentY = 0; currentY < platformSize.Y; currentY++)
            {
                Map[platformSize.X + platformOffset.X, currentY + platformOffset.Y][0].TileType = Tiles.Tile.Type.COLLISION;
                Map[platformOffset.X - 1, currentY + platformOffset.Y][0].TileType = Tiles.Tile.Type.COLLISION;

                Map[platformSize.X + platformOffset.X, currentY + platformOffset.Y][0].TileIndex = 3;
                Map[platformOffset.X - 1, currentY + platformOffset.Y][0].TileIndex = 3;
            }
        }

        // Uses the TileSprite enum and the EnumExtensions class to randomize tiles on the platform in the game
        public void RandomizeTiles(bool randomizePlatform, bool randomizeSafeTile)
        {
            if (randomizePlatform)
            {
                for (int currentX = 0; currentX < platformSize.X; currentX++)
                {
                    for (int currentY = 0; currentY < platformSize.Y; currentY++)
                    {
                        Enum randomTile = typeof(TileTypes).GetRandomEnumValue(); // Uses the EnumExtensions class to get a random element from TileTypes

                        Map[currentX + platformOffset.X, currentY + platformOffset.Y][1].TileIndex = (int)(TileTypes)Enum.Parse(typeof(TileTypes), randomTile.ToString(), true);
                    }
                }
            }

            if (randomizeSafeTile)
            {
                safeTile = typeof(TileTypes).GetRandomEnumValue();

                Map[safeTileOffset.X, safeTileOffset.Y][1].TileIndex = (int)(TileTypes)safeTile;
            }
        }

        // Controls which tiles should be visible on the screen 
        public void TileVisible(bool platformVisible, bool safeTileVisible, bool dangerousTilesVisible)
        {
            for (int currentX = 0; currentX < platformSize.X; currentX++)
            {
                for (int currentY = 0; currentY < platformSize.Y; currentY++)
                {
                    if (platformVisible)
                        Map[currentX + platformOffset.X, currentY + platformOffset.Y][2].TileIndex = 0;
                    else
                        Map[currentX + platformOffset.X, currentY + platformOffset.Y][2].TileIndex = 2;

                    if (CurrentStage == Stage.EXECUTE && !dangerousTilesVisible && Map[currentX + platformOffset.X, currentY + platformOffset.Y][1].TileIndex != (int)(TileTypes)safeTile)
                        Map[currentX + platformOffset.X, currentY + platformOffset.Y][2].TileIndex = 2;
                }
            }

            if (safeTileVisible)
                Map[safeTileOffset.X, safeTileOffset.Y][2].TileIndex = 0;
            else
                Map[safeTileOffset.X, safeTileOffset.Y][2].TileIndex = 2;
        }

        private TimeSpan lastStageChange = TimeSpan.Zero;
        private TimeSpan lastVisualized = TimeSpan.Zero;

        public void StageHandler(GameTime gameTime, Player[] players)
        {
            if (gameTime.TotalGameTime > TimeSpan.FromSeconds(stageTimeSpan[(int)CurrentStage]) + lastStageChange)
            {
                CurrentStage = CurrentStage.NextEnum(); // Uses the EnumExtensions class to get the next Stage element

                lastStageChange = gameTime.TotalGameTime;
            }

            // Sets the actions for each stage
            switch (CurrentStage)
            {
                case Stage.START:
                    TileVisible(false, false, false);

                    roundOver = false;
                    break;

                case Stage.RANDOMIZE:
                    RandomizeTiles(true, false);

                    lastVisualized = gameTime.TotalGameTime;
                    break;

                case Stage.VISUALIZE:
                    if (gameTime.TotalGameTime > TimeSpan.FromSeconds(5) + lastVisualized)
                    {
                        RandomizeTiles(false, true);
                        TileVisible(false, true, false);
                    }
                    else
                        TileVisible(true, false, true);
                    break;

                case Stage.POSITION:
                    break;

                // Kills all the players who are standing on an unsafe tile
                case Stage.EXECUTE:
                    TileVisible(true, true, false);

                    for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                    {
                        Player player = players[currentPlayerIndex];

                        if (Map[(int)player.CurrentPosition.X, (int)player.CurrentPosition.Y][1].TileIndex != (int)(TileTypes)safeTile && player.Active)
                        {
                            player.Active = false;
                            ExecutionOrder.Add(player);
                        }

                        if (player.Active && !roundOver)
                            timesLived[currentPlayerIndex]++;
                    }

                    roundOver = true;
                    break;

                default:
                    break;
            }
        }

        public void EndGame(GameTime gameTime, Player[] players)
        {
            List<Placement> placements = new List<Placement>();

            if (ExecutionOrder.Count == Main.Instance.TotalPlayers && roundOver)
            {
                ExecutionOrder.Reverse();

                for (int currentPlacementIndex = 0; currentPlacementIndex < ExecutionOrder.Count; currentPlacementIndex++)
                {
                    placements.Add(new Placement()
                    {
                        PlayerIndex = (int)ExecutionOrder[currentPlacementIndex].MovementSet,
                        PlayerPlacement = currentPlacementIndex + 1,
                        PlayerText = timesLived[(int)ExecutionOrder[currentPlacementIndex].MovementSet].ToString()
                    });
                }

                Main.Instance.ChangeGameState(new EndScreen(placements, "Quackory"));
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];
                currentPlayer.Draw(spriteBatch);
            }
        }
    }
}
