// MazeOut.cs
// Author Ossian Stange
// LBS Kreativa Gymnasiet

using HonccaFest.Files;
using HonccaFest.MainClasses;
using HonccaFest.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.GameStates
{
    class MazeOut : GameState
    {
        private TimeSpan totalGameDuration = TimeSpan.FromSeconds(100);
        private TimeSpan[] playerFinishTime;

        private List<Placement> placements = new List<Placement>();

        private bool[] playerHasFinished;

        private readonly Vector2[] spawnPoints = new Vector2[]
        {
            new Vector2(Globals.GameSize.X / 2 - 1, Globals.GameSize.Y / 2 - 1),
            new Vector2(Globals.GameSize.X / 2, Globals.GameSize.Y / 2 - 1),
            new Vector2(Globals.GameSize.X / 2 - 1, Globals.GameSize.Y / 2),
            new Vector2(Globals.GameSize.X / 2, Globals.GameSize.Y / 2)
        };

        public MazeOut() : base("MazeOut") { }

        public override void Initialize(ref Player[] players)
        {
            playerFinishTime = new TimeSpan[players.Length];
            playerHasFinished = new bool[players.Length];

            Main.MusicHandler.Play("lose_stinger");

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                players[currentPlayerIndex].ForceMove(spawnPoints[currentPlayerIndex]);

                // Force players that are not in game to lose
                if (!MonoArcade.PlayerIsIngame(currentPlayerIndex))
                {
                    playerFinishTime[currentPlayerIndex] = TimeSpan.FromMinutes(60);
                    playerHasFinished[currentPlayerIndex] = true;
                }
                else
                    playerHasFinished[currentPlayerIndex] = false;
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

        private TimeSpan lastGame = TimeSpan.Zero;
        private GameTime currentGameTime;

        public override void Update(GameTime gameTime, Player[] players)
        {
            if (lastGame == TimeSpan.Zero)
                lastGame = gameTime.TotalGameTime;

            currentGameTime = gameTime;

            if (gameTime.TotalGameTime < lastGame + totalGameDuration)
            {
                for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                {
                    players[currentPlayerIndex].Update(gameTime, Map);

                    if (players[currentPlayerIndex].CurrentPosition == new Vector2(31, 14))
                    {
                        playerFinishTime[currentPlayerIndex] = currentGameTime.TotalGameTime;
                        playerHasFinished[currentPlayerIndex] = true;
                        new AudioEffect("finish_sound").Play(0.2f, players[currentPlayerIndex].CurrentPosition);

                        switch (currentPlayerIndex)
                        {
                            case 0:
                                players[0].Move(gameTime, new Vector2(30, 14), Map);
                                break;
                            case 1:
                                players[1].Move(gameTime, new Vector2(30, 15), Map);
                                break;
                            case 2:
                                players[2].Move(gameTime, new Vector2(29, 15), Map);
                                break;
                            case 3:
                                players[3].Move(gameTime, new Vector2(29, 16), Map);
                                break;
                        }

                        players[currentPlayerIndex].MovementEnabled = false;
                    }

                    if (AllPlayersHaveFinished(playerHasFinished))
                        gameTime.TotalGameTime = lastGame + totalGameDuration;
                }
            }
            else
            {
                // Using ConnectPlayerTime struct from DuckTag.cs
                List<ConnectPlayerTime> temporaryFinishTimes = new List<ConnectPlayerTime>();

                for (int currentPlayerIndex = 0; currentPlayerIndex < playerFinishTime.Length; currentPlayerIndex++)
                {
                    temporaryFinishTimes.Add(new ConnectPlayerTime()
                    {
                        PlayerIndex = currentPlayerIndex,
                        PlayerTime = playerHasFinished[currentPlayerIndex] ? playerFinishTime[currentPlayerIndex] : TimeSpan.FromMinutes(60)
                    });

                    players[currentPlayerIndex].MovementEnabled = true;
                }

                temporaryFinishTimes = temporaryFinishTimes.OrderBy(finish => finish.PlayerTime).ToList();

                for (int currentPlacement = 0; currentPlacement < temporaryFinishTimes.Count; currentPlacement++)
                {
                    ConnectPlayerTime playerTime = temporaryFinishTimes[currentPlacement];

                    placements.Add(new Placement()
                    {
                        PlayerIndex = playerTime.PlayerIndex,
                        PlayerPlacement = currentPlacement + 1,
                        PlayerText = playerHasFinished[playerTime.PlayerIndex] ? $"{Math.Floor(playerTime.PlayerTime.TotalSeconds - lastGame.TotalSeconds)}s" : "DNF"
                    });
                }

                Main.Instance.ChangeGameState(new EndScreen(placements, "MazeOut"));
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            for (int currentX = 0; currentX < Globals.GameSize.X; currentX++)
            {
                for (int currentY = 0; currentY < Globals.GameSize.Y; currentY++)
                {
                    bool isTileFilled = true;

                    Vector2 currentTile = new Vector2(currentX, currentY);

                    for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
                    {
                        if ((Vector2.Distance(players[playerIndex].CurrentPosition, currentTile) < 3) && players[playerIndex].Active)
                        {
                            isTileFilled = false;
                            break;
                        }
                    }

                    spriteBatch.Draw(Main.GraphicsHandler.GetSprite("FilledRectangle"), new Vector2(currentX * Globals.TileSize.X, currentY * Globals.TileSize.Y), isTileFilled ? Color.Black : Color.Black * 0.4f);
                }
            }

            foreach (Player player in players)
                player.Draw(spriteBatch);

            if (currentGameTime != null)
            {
                string scoreString = (totalGameDuration.TotalSeconds - currentGameTime.TotalGameTime.TotalSeconds + lastGame.TotalSeconds).ToString("0.0");

                Vector2 fontSize = Main.ScoreFont.MeasureString(scoreString);

                spriteBatch.DrawString(Main.ScoreFont, scoreString, new Vector2(Globals.ScreenSize.X / 2 - (fontSize.X / 2), Globals.ScreenSize.Y / 50), Color.White);
            }
        }
    }
}