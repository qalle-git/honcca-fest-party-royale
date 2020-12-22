// UltimateDuckRun.cs
// Author Ossian Stange
// LBS Kreativa Gymnasiet

using HonccaFest.Files;
using HonccaFest.MainClasses;
using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.GameStates
{
    class UltimateDuckRun : GameState
    {
        private readonly List<GameObject> projectiles;

        // Sets the shooting interval for the first part of the map
        private int projectileSpawnCooldown;

        // Sets a fixed shooting interval for the upper part of the map
        private const int upperProjSpawnCooldown = 5000;
        private const int bridgeCycleCooldown = 2500;

        private bool[] hasPlayerFinished = new bool[]
        {
            false,
            false,
            false,
            false
        };

        private List<Placement> placements = new List<Placement>();

        private bool isBridgeUp;

        // Sets the parts of the bridge that will cycle between being up or down
        private readonly Vector2[] bridgeTiles = new Vector2[]
        {
            new Vector2(8, 3),
            new Vector2(8, 4),
            new Vector2(11, 3),
            new Vector2(11, 4),
            new Vector2(14, 3),
            new Vector2(14, 4),
            new Vector2(17, 3),
            new Vector2(17, 4),
            new Vector2(20, 3),
            new Vector2(20, 4),
            new Vector2(23, 3),
            new Vector2(23, 4),
            new Vector2(26, 3),
            new Vector2(26, 4),
        };

        // Sets the players spawnpoints
        private readonly Vector2[] spawnPoints = new Vector2[]
        {
            new Vector2(0, 14),
            new Vector2(0, 15),
            new Vector2(0, 16),
            new Vector2(0, 17)
        };

        public UltimateDuckRun() : base("UltimateDuckRun")
        {
            projectiles = new List<GameObject>();
        }

        public override void Initialize(ref Player[] players)
        {
            projectileSpawnCooldown = 1000;
            isBridgeUp = false;

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                players[currentPlayerIndex].ForceMove(spawnPoints[currentPlayerIndex]);

                if (!players[currentPlayerIndex].Active)
                    hasPlayerFinished[currentPlayerIndex] = true;
            }

            RandomizeBrickWallCollision();
            SetInactivePlayerPlacements(players);

            Main.MusicHandler.Play("engines_revved");
        }

        private TimeSpan lastProjectileSpawn, lastUpperProjectileSpawn, lastBridgeChange = TimeSpan.Zero;
        private int currentPlacement = 0;

        private TimeSpan lastGame = TimeSpan.Zero;

        public override void Update(GameTime gameTime, Player[] players)
        {

            if (lastGame == TimeSpan.Zero)
                lastGame = gameTime.TotalGameTime;

            // Checks if every player hasn't finished
            if (!AllPlayersHaveFinished(hasPlayerFinished))
            {
                for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                {
                    players[currentPlayerIndex].Update(gameTime, Map);

                    // Checks if the player has finished. If so, disables the movement and sets the player placement
                    if ((players[currentPlayerIndex].CurrentPosition.Y == 6 || players[currentPlayerIndex].CurrentPosition.Y == 7) && players[currentPlayerIndex].CurrentPosition.X == 29 && !hasPlayerFinished[currentPlayerIndex])
                    {
                        placements.Add(new Placement()
                        {
                            PlayerIndex = currentPlayerIndex,
                            PlayerPlacement = currentPlacement + 1,
                            PlayerText = $"{(gameTime.TotalGameTime - lastGame).TotalSeconds:0.0}s"
                        });

                        hasPlayerFinished[currentPlayerIndex] = true;

                        currentPlacement++;

                        players[currentPlayerIndex].CurrentPosition = new Vector2(currentPlayerIndex % 2 == 0 ? 31 : 30, currentPlayerIndex < 2 ? 6 : 7);

                        players[currentPlayerIndex].MovementEnabled = false;
                    }
                }

                ProjectileHandler(gameTime);
                ProjectileSpawner(gameTime);
                CollisionCheck(players);
                BridgeHandler(gameTime);
            }
            else
            {
                Main.Instance.ChangeGameState(new EndScreen(placements, "UltimateDuckRun"));

                foreach (Player player in players)
                    player.MovementEnabled = true;
            }
        }

        /// <summary>
        /// Returns true if every player has finished
        /// </summary>
        /// <param name="playerHasFinished">A list with values about which players have finished</param>
        /// <returns></returns>
        public static bool AllPlayersHaveFinished(bool[] playerHasFinished)
        {
            int playersFinished = 0;

            for (int currentValue = 0; currentValue < playerHasFinished.Length; currentValue++)
            {
                if (playerHasFinished[currentValue])
                    playersFinished++;
            }

            return playersFinished >= Globals.MaxPlayers;
        }

        /// <summary>
        /// Handles the behavior of every projectile
        /// </summary>
        /// <param name="gameTime"></param>
        private void ProjectileHandler(GameTime gameTime)
        {
            for (int currentProjIndex = 0; currentProjIndex < projectiles.Count; currentProjIndex++)
            {
                GameObject projectile = projectiles[currentProjIndex];

                // Updates and moves the projectile if it's inside boundaries, and deletes it if out of bounds
                if (projectile.CurrentPosition.Y >= -1)
                    if (!projectile.IsOutOfBounds || projectile.CurrentPosition.Y < Globals.GameSize.Y)
                    {
                        projectile.Move(gameTime, new Vector2(projectile.CurrentPosition.X - 1, projectile.CurrentPosition.Y), Map);

                        projectile.Update(gameTime, Map);
                    }
                    else
                    {
                        projectiles.RemoveAt(currentProjIndex);

                        currentProjIndex--;
                    }
            }
        }

        /// <summary>
        /// Handles the spawn of every projectile
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        private void ProjectileSpawner(GameTime gameTime)
        {
            // Spawns a projectile on a random Y-value in an interval
            if (gameTime.TotalGameTime > TimeSpan.FromMilliseconds(projectileSpawnCooldown) + lastProjectileSpawn)
            {
                Vector2 randomSpawnLocation = new Vector2(Globals.GameSize.X, Globals.RandomGenerator.Next(14, 18));

                Animation projectile = new Animation(Main.GraphicsHandler.GetSprite("FireballSprite"), randomSpawnLocation);

                projectiles.Add(projectile);

                lastProjectileSpawn = gameTime.TotalGameTime;

                // Increases the interval span so that it becomes easier dodging the projectiles
                projectileSpawnCooldown++;
            }

            if (gameTime.TotalGameTime > TimeSpan.FromMilliseconds(upperProjSpawnCooldown) + lastUpperProjectileSpawn)
            {
                Vector2 spawnLocation = new Vector2(Globals.GameSize.X, 0);

                Animation projectile = new Animation(Main.GraphicsHandler.GetSprite("FireballSprite"), spawnLocation);

                projectiles.Add(projectile);

                lastUpperProjectileSpawn = gameTime.TotalGameTime;
            }

            // Sets the projectile animation
            foreach (Animation projectile in projectiles)
            {
                projectile.SetAnimationData(new Point(7, 6), new Point(0, 6), Animation.Direction.RIGHT);

                projectile.CurrentFrame.Y = 6;

                projectile.PixelPerMove = 8;
            }
        }

        /// <summary>
        /// Checks if any player has been hit by a projectile
        /// </summary>
        /// <param name="players">List with every player</param>
        private void CollisionCheck(Player[] players)
        {
            // Checks for each player
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                for (int currentProjIndex = 0; currentProjIndex < projectiles.Count; currentProjIndex++)
                {
                    // Checks for each projectile if any collision occurs. If so, the player´s position resets depending on where on the map they got hit
                    if (Collision.IsColliding(projectiles[currentProjIndex], players[currentPlayerIndex]))
                    {
                        if (players[currentPlayerIndex].CurrentPosition.Y > 3)
                            players[currentPlayerIndex].ForceMove(spawnPoints[currentPlayerIndex]);
                        else
                            players[currentPlayerIndex].ForceMove(new Vector2(2, 1));
                    }
                }

                // Checks for each bridge tile if the player has fallen into the water. If so, it resets its position
                for (int currentBridgeIndex = 0; currentBridgeIndex < bridgeTiles.Length; currentBridgeIndex++)
                {
                    if (players[currentPlayerIndex].CurrentPosition == bridgeTiles[currentBridgeIndex] && !isBridgeUp)
                        players[currentPlayerIndex].ForceMove(new Vector2(29, 3));
                }
            }
        }

        /// <summary>
        /// Forces inactive players to lose
        /// </summary>
        /// <param name="players">List with every player</param>
        private void SetInactivePlayerPlacements(Player[] players)
        {
            int currentLastPlacement = 4;

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                // Checks if the player is inactive, and if so sets them to place last
                if (!players[currentPlayerIndex].Active)
                {
                    placements.Add(new Placement()
                    {
                        PlayerIndex = currentPlayerIndex,
                        PlayerPlacement = currentLastPlacement,
                        PlayerText = "DNF"
                    });

                    // Decreases the last placement so that multiple players can't get the same placement
                    currentLastPlacement--;
                }
            }
        }

        /// <summary>
        /// Cycles between the bridges being up or down
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        private void BridgeHandler(GameTime gameTime)
        {
            if (gameTime.TotalGameTime > TimeSpan.FromMilliseconds(bridgeCycleCooldown) + lastBridgeChange)
            {
                isBridgeUp = !isBridgeUp;

                lastBridgeChange = gameTime.TotalGameTime;
            }
        }

        /// <summary>
        /// Sets a random collision on the stone walls in the middle of the map
        /// </summary>
        private void RandomizeBrickWallCollision()
        {
            // The X-values of every stone wall
            int[] Xvalues = { 5, 9, 13, 17, 21, 25 };

            for (int currentX = 0; currentX < Xvalues.Length; currentX++)
            {
                // Randomizes the Y-value for what tile to not have collision
                int openY = Globals.RandomGenerator.Next(9, 13);

                // Sets collision on every tile of the stone walls
                for (int currentY = 9; currentY <= 12; currentY++)
                {
                    Map[Xvalues[currentX], currentY][2].TileType = Tile.Type.COLLISION;
                }

                // Sets the randomized Y-value of each wall to not have collision
                Map[Xvalues[currentX], openY][2].TileType = Tile.Type.NONE;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            if (isBridgeUp)
            {
                foreach (Vector2 bridgeTile in bridgeTiles)
                    Map[(int)bridgeTile.X, (int)bridgeTile.Y][5].TileIndex = 2;
            }
            else
            {
                foreach (Vector2 bridgeTile in bridgeTiles)
                    Map[(int)bridgeTile.X, (int)bridgeTile.Y][5].TileIndex = 27;
            }

            foreach (GameObject projectile in projectiles)
                projectile.Draw(spriteBatch);

            foreach (Player player in players)
                player.Draw(spriteBatch);
        }
    }
}