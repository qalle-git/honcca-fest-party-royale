using HonccaFest.Files;
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
	struct SackState
	{
		public int SacksStored;
		public int SacksPlaced;
		public int PlayerIndex;
	}

	class DuckyRoad : GameState
	{
		private int buttonSize = 30;

		private SackState[] sackStateArray;

		private readonly List<Car> vehicleList = new List<Car>();

		private List<Sack> sackList = new List<Sack>();
		private List<Home> homeList = new List<Home>();
		private List<Placement> placements = new List<Placement>();

		private readonly Vector2[] spawnPoints = new Vector2[4];
		private readonly TimeSpan[] respawnTimers = new TimeSpan[4];

		public DuckyRoad() : base("DuckyRoad")
		{

		}

		public override void Initialize(ref Player[] players)
		{
			lastTimeRandomIndex = Globals.RandomGenerator.Next(2, 4);
			sackStateArray = new SackState[players.Length];
			HomeSpawner(players);

			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				spawnPoints[currentPlayerIndex] = new Vector2(Globals.GameSize.X / players.Length * currentPlayerIndex + 4, 0);

				Player player = players[currentPlayerIndex];
				player.ForceMove(spawnPoints[currentPlayerIndex]);

				sackStateArray[currentPlayerIndex].PlayerIndex = currentPlayerIndex;

			}
		}

		int lastTimeRandomIndex;

		private readonly List<Road> roadList = new List<Road>()
		{
			new Road()
			{
				RoadX = Globals.GameSize.X + 1,
				RoadY = 3,
				RoadDirection = Animation.Direction.RIGHT
			},
			new Road()
			{
				RoadX = -1,
				RoadY = 6,
				RoadDirection = Animation.Direction.LEFT
			},
			new Road()
			{
				RoadX = Globals.GameSize.X + 1,
				RoadY = 10,
				RoadDirection = Animation.Direction.RIGHT
			},
			new Road()
			{
				RoadX = -1,
				RoadY = 13,
				RoadDirection = Animation.Direction.LEFT
			}
		};

		private struct Road
		{
			public int RoadX;
			public int RoadY;

			public Animation.Direction RoadDirection;
		}

		private Road GetRandomRoad()
		{
			int randomRoad = Globals.RandomGenerator.Next(0, roadList.Count);

			return roadList[randomRoad];
		}

		public override void Update(GameTime gameTime, Player[] players)
		{
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
				players[currentPlayerIndex].Update(gameTime, Map);

			CarHandler(gameTime, players);
			SackHandler(gameTime, players);
			HomeHandler(gameTime, players);
			SpawnHandler(gameTime);

			EndGame(gameTime, players);
		}

		private TimeSpan lastSpawnedCar = TimeSpan.Zero;
		private TimeSpan lastSpawnedSack = TimeSpan.Zero;

		private TimeSpan spawnTimer = TimeSpan.FromSeconds(1);
		private TimeSpan gameTimer = TimeSpan.FromSeconds(60);
		private TimeSpan lastGame = TimeSpan.Zero;
		private TimeSpan countdownTimer;

		private void SpawnHandler(GameTime gameTime)
		{
			if (gameTime.TotalGameTime > lastSpawnedCar + spawnTimer)
			{
				SpawnVehicle();

				lastSpawnedCar = gameTime.TotalGameTime;
			}
		}

		private void SpawnVehicle()
		{
			Road randomRoad = GetRandomRoad();

			Car newVehicle = new Car(Main.GraphicsHandler.GetSprite("CarSprite"), new Vector2(randomRoad.RoadX, randomRoad.RoadY));

			newVehicle.SetAnimationData(new Point(3, 3), new Point(0, 3), randomRoad.RoadDirection, 120, 2);
			newVehicle.CurrentFrame.Y = 1;

			vehicleList.Add(newVehicle);
		}

		private void CarHandler(GameTime gameTime, Player[] players)
		{
			for (int currentVehicleIndex = 0; currentVehicleIndex < vehicleList.Count; currentVehicleIndex++)
			{
				Car car = vehicleList[currentVehicleIndex];

				car.Move(gameTime, new Vector2(car.CurrentPosition.X - (car.CurrentDirection == Animation.Direction.RIGHT ? 1 : -1), car.CurrentPosition.Y), Map);
				car.PixelPerMove = 4;

				car.Update(gameTime, Map);

				if (car.IsOutOfBounds)
				{
					vehicleList.RemoveAt(currentVehicleIndex);

					currentVehicleIndex--;
				}

				for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
				{
					Player player = players[currentPlayerIndex];

					if (Collision.IsColliding(player, car))
					{
						player.ForceMove(spawnPoints[currentPlayerIndex]);
						player.MovementEnabled = false;

						respawnTimers[currentPlayerIndex] = gameTime.TotalGameTime;

						Console.WriteLine($"Player {currentPlayerIndex} lost {sackStateArray[currentPlayerIndex].SacksStored} and now has 0 sacks stored.");

						sackStateArray[currentPlayerIndex].SacksStored = 0;
					}

					if (gameTime.TotalGameTime > respawnTimers[currentPlayerIndex] + TimeSpan.FromSeconds(5))
					{
						player.MovementEnabled = true;
					}

				}
			}

			if (vehicleList.Count < 10)
				SpawnHandler(gameTime);
		}

		public void SackSpawner(GameTime gameTime)
		{
			if (gameTime.TotalGameTime > lastSpawnedSack + TimeSpan.FromSeconds(lastTimeRandomIndex) && sackList.Count < Globals.GameSize.X)
			{
				int randomX = Globals.RandomGenerator.Next(0, Globals.GameSize.X);
				foreach (Sack sack in sackList)
				{
					if (sack.CurrentPosition.X == randomX)
					{
						return;
					}
				}

				Sack newSackObject = new Sack(Main.GraphicsHandler.GetSprite("OutlineRectangle"), new Vector2(randomX, Globals.GameSize.Y));

				lastTimeRandomIndex = Globals.RandomGenerator.Next(2, 4);

				lastSpawnedSack = gameTime.TotalGameTime;

				sackList.Add(newSackObject);

				newSackObject.Move(gameTime, new Vector2(newSackObject.CurrentPosition.X, newSackObject.CurrentPosition.Y - 1), Map);

			}
		}

		public void SackHandler(GameTime gameTime, Player[] players)
		{
			for (int currentSackIndex = 0; currentSackIndex < sackList.Count; currentSackIndex++)
			{
				Sack sack = sackList[currentSackIndex];
				for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
				{
					Player player = players[currentPlayerIndex];

					if (Collision.IsColliding(player, sack) && player.JustPressedActionKey(ArcadeButton.Green))
					{
						if (sackStateArray[currentPlayerIndex].SacksStored < 3)
						{
							sackStateArray[currentPlayerIndex].SacksStored++;

							sackList.RemoveAt(currentSackIndex);
							currentSackIndex--;

						}
					}
				}

				sack.Update(gameTime, Map);
			}

			SackSpawner(gameTime);
		}


		public void HomeSpawner(Player[] players)
		{
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player player = players[currentPlayerIndex];

				Home newHomeObject = new Home(Main.GraphicsHandler.GetSprite("DuckyRoadHome"), new Vector2(Globals.GameSize.X / players.Length * currentPlayerIndex + 4, 0));
				homeList.Add(newHomeObject);
			}

		}

		public void HomeHandler(GameTime gameTime, Player[] players)
		{
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player player = players[currentPlayerIndex];
				Home home = homeList[currentPlayerIndex];

				if (Collision.IsColliding(player, home) && player.JustPressedActionKey(ArcadeButton.Green))
				{
					if (sackStateArray[currentPlayerIndex].SacksStored != 0)
					{
						sackStateArray[currentPlayerIndex].SacksPlaced += sackStateArray[currentPlayerIndex].SacksStored;

						sackStateArray[currentPlayerIndex].SacksStored = 0;
					}
				}
			}
		}

		public void EndGame(GameTime gameTime, Player[] players)
		{
			if (lastGame == TimeSpan.Zero)
			{
				lastGame = gameTime.TotalGameTime;
			}

			countdownTimer = gameTimer - gameTime.TotalGameTime + lastGame;

			if (countdownTimer <= TimeSpan.Zero)
			{
				List<SackState> tempList = sackStateArray.ToList().OrderBy(sack => sack.SacksPlaced).ToList();

				tempList.Reverse();

				for (int currentPlacementIndex = 0; currentPlacementIndex < players.Length; currentPlacementIndex++)
				{
					Player player = players[tempList[currentPlacementIndex].PlayerIndex];

					placements.Add(new Placement()
					{
						PlayerIndex = tempList[currentPlacementIndex].PlayerIndex,
						PlayerPlacement = currentPlacementIndex + 1,
						PlayerText = $"{sackStateArray[(int)player.MovementSet].SacksPlaced} sacks"
					});

				}

				Main.Instance.ChangeGameState(new EndScreen(placements, "DuckyRoad"));
			}
		}

		public override void Draw(SpriteBatch spriteBatch, Player[] players)
		{
			base.Draw(spriteBatch, players);

			string countdownString = countdownTimer.Seconds.ToString();

			spriteBatch.DrawString(Main.ScoreFont, countdownString,
				new Vector2(Globals.GameSize.X * (Globals.TileSize.X - 3f) / 2,
				Globals.GameSize.Y * (Globals.TileSize.Y - 3f) / 2), Color.Aquamarine);

			foreach (Car car in vehicleList)
			{
				car.Draw(spriteBatch);
			}

			foreach (Sack sack in sackList)
			{
				sack.Draw(spriteBatch);
			}

			foreach (Home home in homeList)
			{
				home.Draw(spriteBatch);
			}

			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player currentPlayer = players[currentPlayerIndex];
				currentPlayer.Draw(spriteBatch);
			}

			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
				Player player = players[currentPlayerIndex];

				for (int currentSackIndex = 0; currentSackIndex < sackList.Count; currentSackIndex++)
				{
					Sack sack = sackList[currentSackIndex];

					if (Collision.IsColliding(player, sack))
					{
						spriteBatch.Draw(Main.GraphicsHandler.GetSprite("JoystickButtons"), new Rectangle((int)player.CurrentPixelPosition.X + 50, (int)player.CurrentPixelPosition.Y,
							buttonSize, buttonSize), new Rectangle(47 * 5, 39, 122, 122), Color.White);
					}
				}

				if (player.Active)
				{
					spriteBatch.DrawString(Main.DebugFont, sackStateArray[currentPlayerIndex].SacksPlaced.ToString(),
						new Vector2(spawnPoints[currentPlayerIndex].X * 40 + 100, spawnPoints[currentPlayerIndex].Y * 40), Color.Black);

					spriteBatch.DrawString(Main.DebugFont, sackStateArray[currentPlayerIndex].SacksStored.ToString(),
						new Vector2(player.CurrentPixelPosition.X + 50, player.CurrentPixelPosition.Y), Color.White);
				}

			}
		}

		private class Car : Animation
		{
			public Car(Texture2D texture, Vector2 position) : base(texture, position)
			{

			}
		}

		private class Sack : GameObject
		{
			public Sack(Texture2D texture, Vector2 position) : base(texture, position)
			{

			}
		}

		private class Home : GameObject
		{
			public Home(Texture2D texture, Vector2 position) : base(texture, position)
			{

			}

		}
	}
}