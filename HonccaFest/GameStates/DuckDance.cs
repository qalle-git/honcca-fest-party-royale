using HonccaFest.MainClasses;
using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HonccaFest.GameStates.DuckDance;

namespace HonccaFest.GameStates
{
	class DuckDance : GameState
	{
		public enum Direction
		{
			UP = -90,
			DOWN = 90,
			LEFT = 180,
			RIGHT = 0
		}

		private readonly Vector2[] spawnPoints = new Vector2[]
		{
			new Vector2(5, 4),
			new Vector2(5, 13),
			new Vector2(26, 4),
			new Vector2(26, 13)
		};

		public const int StartingArrowCount = 4;

		public List<Direction> CurrentArrowDirections;
		public int CurrentArrowDirection;

		private Arrow currentArrow;

		public DuckDance() : base("DuckDance")
		{
			CurrentArrowDirections = new List<Direction>();
		}

		public override void Initialize(ref Player[] players)
		{
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player currentPlayer = players[currentPlayerIndex];

				currentPlayer.ForceMove(spawnPoints[currentPlayerIndex]);
			}

			currentArrow = new Arrow(new Vector2(Globals.ScreenSize.X / 2, Globals.ScreenSize.Y / 2));

			AddDirections();

			currentArrow.ChangeDirection(CurrentArrowDirections[CurrentArrowDirection]);
		}

		public override void Update(GameTime gameTime, Player[] players)
		{
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player currentPlayer = players[currentPlayerIndex];

				currentPlayer.Update(gameTime, Map);
			}

			ArrowHandler(gameTime);
		}

		private TimeSpan lastTimeDirection = TimeSpan.Zero;
		private TimeSpan directionChangeTimer = TimeSpan.FromSeconds(1);

		private void ArrowHandler(GameTime gameTime)
		{
			currentArrow.Update(gameTime, Map);

			if (gameTime.TotalGameTime > lastTimeDirection + directionChangeTimer)
			{
				lastTimeDirection = gameTime.TotalGameTime;

				if (CurrentArrowDirection < CurrentArrowDirections.Count - 1)
				{
					CurrentArrowDirection++;

					Direction newDirection = CurrentArrowDirections[CurrentArrowDirection];

					currentArrow.ChangeDirection(newDirection);

					Console.WriteLine($"Changing to new directionIndex {CurrentArrowDirection}");
				} else
				{
					CurrentArrowDirection = 0;

					AddDirections(2);
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch, Player[] players)
		{
			base.Draw(spriteBatch, players);

			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player currentPlayer = players[currentPlayerIndex];

				currentPlayer.Draw(spriteBatch);
			}

			DrawArrow(spriteBatch);
		}

		private void DrawArrow(SpriteBatch spriteBatch)
		{
			currentArrow.Draw(spriteBatch);
		}

		private void AddDirections(int howMany = StartingArrowCount)
		{
			for (int currentArrowDirectionIndex = 0; currentArrowDirectionIndex < howMany; currentArrowDirectionIndex++)
			{
				Direction randomDirection = GetRandomDirection();

				Console.WriteLine($"Randomized a new direction: {randomDirection}");

				CurrentArrowDirections.Add(randomDirection);
			}
		}

		private Direction GetRandomDirection()
		{
			int randomDirectionIndex = Globals.RandomGenerator.Next(0, Enum.GetValues(typeof(Direction)).Length);

			switch (randomDirectionIndex)
			{
				case 0:
					return Direction.UP;
				case 1:
					return Direction.DOWN;
				case 2:
					return Direction.LEFT;
				case 3:
					return Direction.RIGHT;
				default:
					return Direction.UP;
			}
		}

		#region ArrowClass

		private class Arrow : GameObject
		{
			private Point arrowSize = new Point(96, 96);

			public Arrow(Vector2 position) : base(Main.CharacterSelectionArrow, position)
			{
				CurrentPixelPosition = position;
			}

			public override void Update(GameTime gameTime, Tile[,][] map)
			{
				base.Update(gameTime, map);
			}

			public override void Draw(SpriteBatch sb)
			{
				if (!Active)
					return;

				Rectangle drawRectangle = new Rectangle((int)CurrentPixelPosition.X, (int)CurrentPixelPosition.Y, arrowSize.X, arrowSize.Y);

				sb.Draw(Texture, drawRectangle, new Rectangle(0, 0, Texture.Width, Texture.Height), Color.Gray, Rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), CurrentSpriteEffect, 0);
			}

			public void ChangeDirection(Direction direction)
			{
				Rotation = MathHelper.ToRadians((float)direction);
			}
		}

        #endregion
    }
}
