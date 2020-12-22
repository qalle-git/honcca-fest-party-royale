

using HonccaFest.Files;
using HonccaFest.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace HonccaFest.GameStates
{
    class DuckOut : GameState
    {
        private const float endGameTimerInMilliseconds = 60000 * 1.5f;
        private Timer endGameTimer;

        private readonly Vector2[] spawnPoints = new Vector2[]
        {
            new Vector2(10, 4),
            new Vector2(4, 14),
            new Vector2(23, 12),
            new Vector2(25, 4)
        };

        private const int maxPlayerHealth = 5;
        private int[] playerHealth;

        private Timer[] playerInvincibleTimer;

        private readonly List<Placement> placements = new List<Placement>();

        private string timeLeftLabel = $"{endGameTimerInMilliseconds}s";

        public DuckOut() : base("DuckOut")
        {
        }

        public override void Initialize(ref Player[] players)
        {
            playerHealth = new int[players.Length];
            playerInvincibleTimer = new Timer[players.Length];

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                if (currentPlayer.Active)
                {
                    playerHealth[currentPlayerIndex] = maxPlayerHealth;
                    playerInvincibleTimer[currentPlayerIndex] = new Timer(4000, true);

                    currentPlayer.ForceMove(spawnPoints[currentPlayerIndex]);
                } else
                {
                    placements.Add(new Placement()
                    {
                        PlayerIndex = currentPlayerIndex,
                        PlayerPlacement = players.Length - placements.Count,
                        PlayerText = ""
                    });
                }
            }

            Main.MusicHandler.Play("engines_revved");

            endGameTimer = new Timer(endGameTimerInMilliseconds);
        }

        public override void Update(GameTime gameTime, Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                currentPlayer.Update(gameTime, Map);
            }

            timeLeftLabel = $"{(endGameTimer.GetTimeRemaining(gameTime) / 1000).ToString("0.0")}s";

            StunHandler(gameTime, players);

            FinishGameHandler(gameTime, players);
        }

        private void FinishGameHandler(GameTime gameTime, Player[] players)
        {
            if (endGameTimer.IsFinished(gameTime) || PlayersRemaining(players) < 2)
            {
                FinishGame(gameTime, players);
            }
        }

        private void FinishGame(GameTime gameTime, Player[] players)
        {
            Player winner = GetWinner(players);

            placements.Add(new Placement()
            {
                PlayerIndex = (int)winner.MovementSet,
                PlayerPlacement = 1,
                PlayerText = "BEAST"
            });

            Globals.DebugPrint("Finish game.");

            Main.Instance.ChangeGameState(new EndScreen(placements, LevelName));
        }

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            DrawPlayers(spriteBatch, players);

            Vector2 fontSize = Main.ScoreFont.MeasureString(timeLeftLabel);

            spriteBatch.DrawString(Main.ScoreFont, timeLeftLabel, new Vector2(Globals.ScreenSize.X / 2 - fontSize.X / 2, 0), Color.White);
        }

        private void DrawPlayers(SpriteBatch spriteBatch, Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                if (currentPlayer.Active)
                {
                    Rectangle rectangle = currentPlayer.GetRectangle();

                    spriteBatch.DrawString(Main.DebugFont, playerHealth[currentPlayerIndex].ToString(), new Vector2(currentPlayer.CurrentPixelPosition.X + rectangle.Width / 3, currentPlayer.CurrentPixelPosition.Y - rectangle.Height / 4), Color.White);
                }

                currentPlayer.Draw(spriteBatch);
            }
        }

        public override void StunHandler(GameTime gameTime, Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                for (int currentColliderIndex = 0; currentColliderIndex < players.Length; currentColliderIndex++)
                    if (currentPlayerIndex != currentColliderIndex)
                    {
                        Player colliderPlayer = players[currentColliderIndex];

                        // Checks if the player is active and close to another player. If Yellow key is pressed then stun that duck.
                        if (currentPlayer.Active && !currentPlayer.IsStunned && Vector2.Distance(currentPlayer.CurrentPixelPosition, colliderPlayer.CurrentPixelPosition) <= Globals.TileSize.X && playerInvincibleTimer[currentColliderIndex].IsFinished(gameTime) && currentPlayer.JustPressedActionKey(ArcadeButton.Yellow))
                        {
                            Globals.DebugPrint($"{currentPlayerIndex} just punched {currentColliderIndex} {Vector2.Distance(currentPlayer.CurrentPixelPosition, currentPlayer.CurrentPixelPosition)}.");

                            playerHealth[currentColliderIndex]--;
                            playerInvincibleTimer[currentColliderIndex].ResetTimer(gameTime);

                            if (playerHealth[currentColliderIndex] <= 0)
                            {
                                colliderPlayer.Active = false;

                                placements.Add(new Placement()
                                {
                                    PlayerIndex = currentColliderIndex,
                                    PlayerPlacement = players.Length - placements.Count,
                                    PlayerText = "KO"
                                });
                            }

                            colliderPlayer.GetStunned(currentPlayer);
                        }
                    }
            }
        }

        private int PlayersRemaining(Player[] players)
        {
            int playersRemaining = 0;

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                if (currentPlayer.Active)
                    playersRemaining++;
            }

            return playersRemaining;
        }

        private Player GetWinner(Player[] players)
        {
            int lastHealth = 0;
            int lastPlayerIndex = -1;

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                if (currentPlayer.Active && playerHealth[currentPlayerIndex] > lastHealth)
                {
                    lastPlayerIndex = currentPlayerIndex;
                    lastHealth = playerHealth[currentPlayerIndex];
                }
            }

            return players[lastPlayerIndex];
        }
    }
}
