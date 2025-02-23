﻿// Creator.cs
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
    public struct ConnectPlayerTime
    {
        public int PlayerIndex;
        public TimeSpan PlayerTime;
    }

    class DuckTag : GameState
    {
        private int isTagger;

        // Constant values that can't be set to 'const'
        private int tagDistance = Globals.TileSize.X * 2;
        private TimeSpan tagCooldown = TimeSpan.FromMilliseconds(2000);

        private TimeSpan lastTagged = TimeSpan.Zero;
        private TimeSpan totalGameDuration = TimeSpan.FromMinutes(1);
        private TimeSpan[] playerTaggerTime;

        private float taggerArrowY;
        private float taggerArrowDirection;

        private readonly Vector2[] spawnPoints = new Vector2[]
        {
            new Vector2(2, 2),
            new Vector2(9, 14),
            new Vector2(17, 15),
            new Vector2(16, 4)
        };

        public DuckTag() : base("DuckTag")
        {
            taggerArrowY = Globals.GameSize.Y;
            taggerArrowDirection = 1;
        }

        public int GenerateRandomTagger(Player[] players)
        {
            int randomPlayerIndex = Globals.RandomGenerator.Next(0, players.Length);

            if (!players[randomPlayerIndex].Active)
                return GenerateRandomTagger(players);
            else
                return randomPlayerIndex;
        }

        public override void Initialize(ref Player[] players)
        {
            playerTaggerTime = new TimeSpan[players.Length];

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                players[currentPlayerIndex].ForceMove(spawnPoints[currentPlayerIndex]);
                    
                // Force players that arent in the game to lose.
                if (!MonoArcade.PlayerIsIngame(currentPlayerIndex))
                    playerTaggerTime[currentPlayerIndex] = TimeSpan.FromMinutes(60);
            }

            isTagger = GenerateRandomTagger(players);

            Main.MusicHandler.Play("duck_tag");
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
                Player tagger = players[isTagger];

                if (!tagger.ChangingTile)
                    tagger.PixelPerMove = 4;

                taggerArrowY += taggerArrowDirection;

                if (taggerArrowY < Globals.GameSize.X)
                    taggerArrowDirection = 0.4f;
                if (taggerArrowY > Globals.GameSize.X * 1.3)
                    taggerArrowDirection = -0.4f;

                playerTaggerTime[isTagger] += gameTime.ElapsedGameTime;

                if (tagger.IsUsingActionKey(ArcadeButton.Green) && gameTime.TotalGameTime > lastTagged + tagCooldown)
                {
                    for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
                    {
                        if (Vector2.Distance(tagger.CurrentPixelPosition, players[playerIndex].CurrentPixelPosition) < tagDistance && playerIndex != isTagger && players[playerIndex].Active)
                        {
                            tagger.PixelPerMove = 2;
                            isTagger = playerIndex;
                            players[isTagger].MovementEnabled = false;
                            lastTagged = gameTime.TotalGameTime;
                            new AudioEffect("quack_sound").Play(0.2f, tagger.CurrentPosition);

                            break;
                        }
                    }
                }

                if (gameTime.TotalGameTime > lastTagged + tagCooldown)
                    players[isTagger].MovementEnabled = true;

                for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                    players[currentPlayerIndex].Update(gameTime, Map);
            }
            else
            {
                List<Placement> placements = new List<Placement>();
                List<ConnectPlayerTime> temporaryTagTimes = new List<ConnectPlayerTime>();

                for (int currentPlayerIndex = 0; currentPlayerIndex < playerTaggerTime.Length; currentPlayerIndex++)
                {
                    temporaryTagTimes.Add(new ConnectPlayerTime()
                    {
                        PlayerIndex = currentPlayerIndex,
                        PlayerTime = playerTaggerTime[currentPlayerIndex]
                    });

                    players[currentPlayerIndex].PixelPerMove = 2;
                    players[currentPlayerIndex].MovementEnabled = true;
                }

                temporaryTagTimes = temporaryTagTimes.OrderBy(tag => tag.PlayerTime).ToList();

                for (int currentPlacement = 0; currentPlacement < temporaryTagTimes.Count; currentPlacement++)
                {
                    ConnectPlayerTime playerTime = temporaryTagTimes[currentPlacement];

                    placements.Add(new Placement()
                    {
                        PlayerIndex = playerTime.PlayerIndex,
                        PlayerPlacement = currentPlacement + 1,
                        PlayerText = $"{Math.Floor(playerTime.PlayerTime.TotalSeconds)}s"
                    });
                }

                Main.Instance.ChangeGameState(new EndScreen(placements, "DuckTag"));
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                if (playerIndex == isTagger)
                {
                    spriteBatch.Draw(Main.GraphicsHandler.GetSprite("TaggerArrow"), new Vector2(players[isTagger].CurrentPixelPosition.X, players[isTagger].CurrentPixelPosition.Y - taggerArrowY), Color.Red);
                }
            }

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].Draw(spriteBatch);

            if (currentGameTime != null)
            {
                string scoreString = (totalGameDuration.TotalSeconds - currentGameTime.TotalGameTime.TotalSeconds + lastGame.TotalSeconds).ToString("0.0");

                Vector2 fontSize = Main.ScoreFont.MeasureString(scoreString);

                spriteBatch.DrawString(Main.ScoreFont, scoreString, new Vector2(Globals.ScreenSize.X / 2 - (fontSize.X / 2), Globals.ScreenSize.Y / 50), Color.White);
            }
        }
    }
}
