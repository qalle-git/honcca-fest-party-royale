// Creator.cs
// Author Ossian Stange
// LBS Kreativa Gymnasiet

using HonccaFest.Files;
using HonccaFest.MainClasses;
using HonccaFest.Sound;
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
    struct ConnectPlayerScore
    {
        public int PlayerIndex;
        public int PlayerScore;
    }

    class Coin : Animation
    {
        public Coin(Texture2D texture, Vector2 position) : base(texture, position)
        {
        }

        public void SetRandomPosition(Tile[,][] _map, Coin[] _coins)
        {
            int randomX = Globals.RandomGenerator.Next(0, Globals.GameSize.X);
            int randomY = Globals.RandomGenerator.Next(0, Globals.GameSize.Y);

            Tile[] tiles = _map[randomX, randomY];

            for (int currentTileIndex = 0; currentTileIndex < tiles.Length; currentTileIndex++)
            {
                Tile currentTile = tiles[currentTileIndex];

                if (currentTile.TileType == Tile.Type.COLLISION)
                {
                    SetRandomPosition(_map, _coins);
                    return;
                }
            }

            ForceMove(new Vector2(randomX, randomY));
        }
    }

    class QuackCash : GameState
    {
        private int coinAmount;

        private int[] coinsCollected;
        private Coin[] coins;

        private AudioEffect coinSound;

        private TimeSpan totalGameDuration = TimeSpan.FromMinutes(1);

        private readonly Vector2[] spawnPoints = new Vector2[]
        {
            new Vector2(6, 6),
            new Vector2(9, 14),
            new Vector2(17, 16),
            new Vector2(12, 3)
        };

        public QuackCash() : base("QuackCash")
        {

        }

        public override void Initialize(ref Player[] players)
        {
            coinAmount = 15;

            coinSound = new AudioEffect("coin_sound");

            coinsCollected = new int[players.Length];

            coins = new Coin[coinAmount];

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].ForceMove(spawnPoints[currentPlayerIndex]);

            for (int i = 0; i < coins.Length; i++)
            {
                coins[i] = new Coin(Main.GraphicsHandler.GetSprite("CoinSprite"), Vector2.Zero);
                coins[i].SetRandomPosition(Map, coins);
                coins[i].CurrentState = Animation.State.ANIMATING;
                coins[i].SetAnimationData(new Point(5, 0), new Point(0, 5), Animation.Direction.RIGHT);
            }

            Main.MusicHandler.Play("engines_revved");
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
                    for (int currentCoinIndex = 0; currentCoinIndex < coins.Length; currentCoinIndex++)
                    {
                        if (players[currentPlayerIndex].CurrentPosition == coins[currentCoinIndex].CurrentPosition && players[currentPlayerIndex].JustPressedActionKey(ArcadeButton.Green))
                        {
                            coins[currentCoinIndex].SetRandomPosition(Map, coins);
                            coinsCollected[currentPlayerIndex]++;

                            coinSound.Play(0.2f, coins[currentCoinIndex].CurrentPosition);
                        }

                        coins[currentCoinIndex].Update(gameTime, Map);
                    }
                }
            }
            else
            {
                List<Placement> placements = new List<Placement>();
                List<ConnectPlayerScore> tempPlayerScores = new List<ConnectPlayerScore>();

                for (int currentPlayer = 0; currentPlayer < players.Length; currentPlayer++)
                {
                    tempPlayerScores.Add(new ConnectPlayerScore()
                    {
                        PlayerIndex = currentPlayer,
                        PlayerScore = coinsCollected[currentPlayer]
                    });
                }

                tempPlayerScores = tempPlayerScores.OrderBy(score => score.PlayerScore).ToList();
                tempPlayerScores.Reverse();

                for (int currentPlacement = 0; currentPlacement < tempPlayerScores.Count; currentPlacement++)
                {
                    ConnectPlayerScore playerScore = tempPlayerScores[currentPlacement];

                    placements.Add(new Placement()
                    {
                        PlayerIndex = playerScore.PlayerIndex,
                        PlayerPlacement = currentPlacement + 1,
                        PlayerText = $"{playerScore.PlayerScore}"
                    });
                }

                Main.Instance.ChangeGameState(new EndScreen(placements, "QuackCash"));
            }

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                players[currentPlayerIndex].Update(gameTime, Map);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            for (int currentCoinIndex = 0; currentCoinIndex < coins.Length; currentCoinIndex++)
                coins[currentCoinIndex].Draw(spriteBatch);

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                players[currentPlayerIndex].Draw(spriteBatch);

                string displayScore = $"Player {currentPlayerIndex + 1}: {coinsCollected[currentPlayerIndex]}";

                if (players[currentPlayerIndex].Active)
                    spriteBatch.DrawString(Main.ScoreFont, displayScore,
                        new Vector2(currentPlayerIndex % 2 == 0 ? 10 : Globals.ScreenSize.X - Main.ScoreFont.MeasureString(displayScore).X - 10, currentPlayerIndex < 2 ? 10 : Globals.ScreenSize.Y - Main.ScoreFont.MeasureString(displayScore).Y - 10),
                        Color.Red);
            }

            if (currentGameTime != null)
            {
                string scoreString = (totalGameDuration.TotalSeconds - currentGameTime.TotalGameTime.TotalSeconds + lastGame.TotalSeconds).ToString("0.0");

                Vector2 fontSize = Main.ScoreFont.MeasureString(scoreString);

                spriteBatch.DrawString(Main.ScoreFont, scoreString, new Vector2(Globals.ScreenSize.X / 2 - (fontSize.X / 2), Globals.ScreenSize.Y / 50), Color.White);
            }
        }
    }
}