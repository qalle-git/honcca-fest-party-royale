﻿using HonccaFest.MainClasses;
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

        // Constant values
        private int tagDistance = Globals.TileSize.X * 2;
        private TimeSpan tagCooldown = TimeSpan.FromMilliseconds(2000);
        private TimeSpan lastTagged = TimeSpan.Zero;

        public DuckTag() : base("DuckTag")
        {

        }

        public override void Initialize(ref Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].ForceMove(new Vector2(currentPlayerIndex * players.Length, 5));

            int randomPlayerIndex = Globals.RandomGenerator.Next(0, players.Length);
            isTagger = randomPlayerIndex;
        }

        public override void Update(GameTime gameTime, Player[] players)
        {
            Player tagger = players[isTagger];

            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                if (Vector2.Distance(tagger.CurrentPixelPosition, players[playerIndex].CurrentPixelPosition) < tagDistance && )
                {
                    isTagger = playerIndex;
                }
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
