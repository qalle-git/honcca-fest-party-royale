using HonccaFest.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.GameStates
{
    class DuckTag : GameState
    {
        private int isTagger;

        // Constant values that can't be set to 'const'
        private int tagDistance = Globals.TileSize.X * 2;
        private TimeSpan tagCooldown = TimeSpan.FromMilliseconds(2000);

        private TimeSpan lastTagged = TimeSpan.Zero;
        private TimeSpan totalGameDuration = TimeSpan.FromMinutes(1);
        private TimeSpan currentGameDuration = TimeSpan.Zero;
        private TimeSpan[] playerTaggerTime;

        public DuckTag() : base("DuckTag")
        {
        }

        public override void Initialize(ref Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].ForceMove(new Vector2(currentPlayerIndex * players.Length, 5));

            int randomPlayerIndex = Globals.RandomGenerator.Next(0, players.Length);
            isTagger = randomPlayerIndex;

            playerTaggerTime = new TimeSpan[players.Length];
        }

        public override void Update(GameTime gameTime, Player[] players)
        {
            currentGameDuration = gameTime.TotalGameTime;

            if (currentGameDuration < totalGameDuration)
            {
                Player tagger = players[isTagger];

                if (!tagger.ChangingTile)
                    tagger.PixelPerMove = 4;

                playerTaggerTime[isTagger] += gameTime.ElapsedGameTime;

                if (tagger.IsUsingActionKey(4) && gameTime.TotalGameTime > lastTagged + tagCooldown)
                {
                    for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
                    {
                        if (Vector2.Distance(tagger.CurrentPixelPosition, players[playerIndex].CurrentPixelPosition) < tagDistance && playerIndex != isTagger)
                        {
                            tagger.PixelPerMove = 2;
                            isTagger = playerIndex;
                            lastTagged = gameTime.TotalGameTime;

                            break;
                        }
                    }
                }
            }
            else
            {
                // play endscreen
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                if (playerIndex == isTagger)
                {
                    spriteBatch.Draw(Main.OutlineRectangle, players[isTagger].CurrentPixelPosition, Color.Red);
                }
            }

            base.Draw(spriteBatch, players);
        }
    }
}
