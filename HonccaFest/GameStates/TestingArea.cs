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
		public enum State
		{
			SHOW = 2000,
			HIDE = 5000,
			REVEAL = 4000,
			GIVE = 2500
		}

		public State CurrentState = State.SHOW;

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

			if (gameTime.TotalGameTime > TimeSpan.FromMilliseconds((int)CurrentState))
			{
				CurrentState = GetNextState();
			}
		}

		private State GetNextState()
		{
			return (State)Array.IndexOf(Enum.GetValues(CurrentState.GetType()), CurrentState);
		}

		public override void Draw(SpriteBatch spriteBatch, Player[] players)
		{
			base.Draw(spriteBatch, players);
		}
	}
}
