using HonccaFest.MainClasses;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.GameStates
{
    class Quackory : GameState
    {
        public Quackory() : base("Quackory")
        {

        }

        public override void Initialize(ref Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].ForceMove(new Vector2(currentPlayerIndex * players.Length, 5));
        }

        public override void Update(GameTime gameTime, Player[] players)
        {
            
        }
    }
}
