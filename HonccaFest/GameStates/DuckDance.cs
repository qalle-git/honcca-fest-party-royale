// DuckDance.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using HonccaFest.Files;
using HonccaFest.MainClasses;
using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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
			new Vector2(5, 12),
			new Vector2(24, 4),
			new Vector2(24, 12)
		};

		public const int StartingArrowCount = 3;

		public List<Direction> CurrentArrowDirections;
		public int CurrentArrowDirection;

		public bool ShowcasingRoute = true;

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

			currentArrow = new Arrow(new Vector2(Globals.ScreenSize.X / 2, Globals.ScreenSize.Y / 2)) { Active = false };

			AddDirections();

			currentArrow.ChangeDirection(CurrentArrowDirections[CurrentArrowDirection]);

			TogglePlayerMovement(players, false);
		}

		public override void Update(GameTime gameTime, Player[] players)
		{
			if (StartedGameState == TimeSpan.Zero)
				StartedGameState = gameTime.TotalGameTime;

			PlayerHandler(gameTime, players);

			ArrowHandler(gameTime, players);
		}

		private void PlayerHandler(GameTime gameTime, Player[] players)
		{
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player currentPlayer = players[currentPlayerIndex];

				if (currentPlayer.Active)
					currentPlayer.Update(gameTime, Map);
			};
		}

		private TimeSpan lastTimeDirection = TimeSpan.Zero;
		private TimeSpan directionChangeTimer = TimeSpan.FromSeconds(2);

		private void ArrowHandler(GameTime gameTime, Player[] players)
		{
			if (gameTime.TotalGameTime > StartedGameState + TimeSpan.FromSeconds(3))
			{
				if (!currentArrow.Active)
					currentArrow.Active = true;

				currentArrow.Update(gameTime, Map);

				if (gameTime.TotalGameTime > lastTimeDirection + directionChangeTimer)
				{
					lastTimeDirection = gameTime.TotalGameTime;

					CheckForDestruction(gameTime, players);

					if (CurrentArrowDirection < CurrentArrowDirections.Count - 1)
					{
						CurrentArrowDirection++;
					}
					else
					{
						if (ShowcasingRoute)
						{
							ShowcasingRoute = false;

							TogglePlayerMovement(players, true);
						} 
						else
						{
							ResetPositions(players);

							AddDirections(2);
						}

						CurrentArrowDirection = 0;
					}

					Direction newDirection = CurrentArrowDirections[CurrentArrowDirection];

					currentArrow.ChangeDirection(newDirection);
				}
			}
		}

		private void TogglePlayerMovement(Player[] players, bool toggle)
		{
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
				players[currentPlayerIndex].MovementEnabled = toggle;
		}

		private void CheckForDestruction(GameTime gameTime, Player[] players)
		{
			if (ShowcasingRoute)
				return;

			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player currentPlayer = players[currentPlayerIndex];

				if (!IsPlayerSafe(players, currentPlayerIndex))
					currentPlayer.Active = false;
			}
		}

		public override void Draw(SpriteBatch spriteBatch, Player[] players)
		{
			base.Draw(spriteBatch, players);

			DrawPlayers(spriteBatch, players);
			DrawArrow(spriteBatch);
		}

		private void DrawPlayers(SpriteBatch spriteBatch, Player[] players)
		{
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player currentPlayer = players[currentPlayerIndex];

				spriteBatch.DrawString(Main.DebugFont, $"Pos{currentPlayer.CurrentPosition} Safe{GetSafePosition(currentPlayerIndex)}", currentPlayer.CurrentPixelPosition, Color.White);

				currentPlayer.Draw(spriteBatch);
			}
		}

		private void DrawArrow(SpriteBatch spriteBatch)
		{
			if (ShowcasingRoute)
				currentArrow.Draw(spriteBatch);
		}

		private void ResetPositions(Player[] players)
		{
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
				players[currentPlayerIndex].ForceMove(spawnPoints[currentPlayerIndex]);
		}

		/// <summary>
		/// Add directions to the dance.
		/// </summary>
		/// <param name="amount">How many directions you want.</param>
		private void AddDirections(int amount = StartingArrowCount)
		{
			for (int currentArrowDirectionIndex = 0; currentArrowDirectionIndex < amount; currentArrowDirectionIndex++)
			{
				Direction randomDirection = GetRandomDirection();

				Console.WriteLine($"Randomized a new direction: {randomDirection}");

				CurrentArrowDirections.Add(randomDirection);
			}
		}

		private Direction GetRandomDirection()
		{
			Direction newDirection;

			switch (Globals.RandomGenerator.Next(0, Enum.GetValues(typeof(Direction)).Length))
			{
				case 0:
					newDirection = Direction.UP;

					break;
				case 1:
					newDirection = Direction.DOWN;

					break;
				case 2:
					newDirection = Direction.LEFT;

					break;
				case 3:
					newDirection = Direction.RIGHT;

					break;
				default:
					newDirection = Direction.UP;

					break;
			}

			Vector2 safePosition = GetSafePosition(0, true);
			Vector2 arrowCoordinate = DirectionToCoordinate(newDirection);

			Vector2 newCoordinate = safePosition + arrowCoordinate;

			Console.WriteLine($"{safePosition} {arrowCoordinate} {newCoordinate}");

			Console.WriteLine(Collision.TilesHasCollision(Map[(int)newCoordinate.X, (int)newCoordinate.Y]));
			Console.WriteLine(Map[(int)newCoordinate.X, (int)newCoordinate.Y][0].TileType);

			if (Collision.TilesHasCollision(Map[(int)newCoordinate.X, (int)newCoordinate.Y]))
				return GetRandomDirection();

			return newDirection;
		}

		private bool IsPlayerSafe(Player[] players, int playerIndex)
		{
			Vector2 safePosition = GetSafePosition(playerIndex);

			return players[playerIndex].CurrentPosition == safePosition;
		}

		private Vector2 GetSafePosition(int playerIndex, bool overrideCurrent = false)
		{
			Vector2 arrowPosition = spawnPoints[playerIndex];

			for (int currentDirectionIndex = 0; currentDirectionIndex < CurrentArrowDirections.Count; currentDirectionIndex++)
			{
				if (overrideCurrent || currentDirectionIndex <= CurrentArrowDirection)
				{
					Vector2 addVector = DirectionToCoordinate(CurrentArrowDirections[currentDirectionIndex]);

					arrowPosition += addVector;
				}
			}

			return arrowPosition;
		}

		public Vector2 DirectionToCoordinate(Direction direction)
		{
			Vector2 returnVector = Vector2.Zero;

			switch (direction)
			{
				case Direction.UP:
					returnVector.Y--;

					break;
				case Direction.DOWN:
					returnVector.Y++;

					break;
				case Direction.LEFT:
					returnVector.X--;

					break;
				case Direction.RIGHT:
					returnVector.X++;

					break;
				default:
					break;
			}

			return returnVector;
		}

		#region ArrowClass

		private class Arrow : GameObject
		{
			private Point arrowSize = new Point(96, 96);

			public Arrow(Vector2 position) : base(Main.GraphicsHandler.GetSprite("CharacterSelectionArrow"), position)
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