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
	class TestingArea : GameState
	{
		public TestingArea() : base("TestingArea")
		{

		}

		public override void Initialize(ref Player[] players)
		{
		}

		public override void Update(GameTime gameTime, Player[] players)
		{
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player currentPlayer = players[currentPlayerIndex];

				currentPlayer.Update(gameTime, Map);
			}
		}

		int secondsRemaining = 297;

		public override void Draw(SpriteBatch spriteBatch, Player[] players)
		{
			base.Draw(spriteBatch, players);

			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player currentPlayer = players[currentPlayerIndex];

				currentPlayer.Draw(spriteBatch);
			}

			int minutesLeft = secondsRemaining / 60;
			int secondsLeft = secondsRemaining % 60;

			secondsRemaining--;

			Console.WriteLine($"{(minutesLeft < 10 ? "0" : "")}{minutesLeft}:{(secondsLeft < 10 ? "0" : "")}{secondsLeft}");
		}
	}
}
