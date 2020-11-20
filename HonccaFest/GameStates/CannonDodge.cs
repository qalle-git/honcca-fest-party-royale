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
    class CannonDodge : GameState
    {
        private readonly List<GameObject> fireballObjects;

        private float fireballSpawnCooldown = 250;

        private readonly List<Placement> placements = new List<Placement>();

        private readonly Vector2[] spawnPoints = new Vector2[]
        {
            new Vector2(5, 15),
            new Vector2(11, 17),
            new Vector2(19, 16),
            new Vector2(30, 15)
        };

        public CannonDodge() : base("CannonDodge")
        {
            fireballObjects = new List<GameObject>();
        }

        public override void Initialize(ref Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                currentPlayer.ForceMove(spawnPoints[currentPlayerIndex]);

                currentPlayer.MovementEnabled = true;
            }
        }

        private TimeSpan lastFireballSpawn = TimeSpan.Zero;

        public override void Update(GameTime gameTime, Player[] players)
        {
            FireballSpawner(gameTime);
            FireballHandler(gameTime);

            CollisionCheck(players);

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].Update(gameTime, Map);
        }

		private void FireballHandler(GameTime gameTime)
		{
            for (int currentFireballIndex = 0; currentFireballIndex < fireballObjects.Count; currentFireballIndex++)
            {
                GameObject fireball = fireballObjects[currentFireballIndex];

                if (fireball.CurrentPosition.Y >= -1)
                    if (!fireball.IsOutOfBounds || fireball.CurrentPosition.Y < Globals.GameSize.Y)
                    {
                        fireball.Move(gameTime, new Vector2(fireball.CurrentPosition.X, fireball.CurrentPosition.Y + 1), Map);

                        fireball.Update(gameTime, Map);
                    }
                    else
                    {
                        fireballObjects.RemoveAt(currentFireballIndex);

                        currentFireballIndex--;
                    }
            }
        }

		private void FireballSpawner(GameTime gameTime)
		{
            if (gameTime.TotalGameTime > TimeSpan.FromMilliseconds(fireballSpawnCooldown) + lastFireballSpawn)
            {
                Vector2 randomSpawn = new Vector2(Globals.RandomGenerator.Next(0, Map.GetLength(0)), -1);

                Animation fireballObject = new Animation(Main.FireballSprite, randomSpawn);

                fireballObject.SetAnimationData(new Point(7, 6), new Point(0, 6), Animation.Direction.RIGHT);

                fireballObject.CurrentFrame.Y = 6;

                fireballObject.PixelPerMove = 8;

                fireballObjects.Add(fireballObject);

                lastFireballSpawn = gameTime.TotalGameTime;

                fireballSpawnCooldown -= 2f;
            }

            fireballSpawnCooldown = MathHelper.Clamp(fireballSpawnCooldown, 50, 1000);
        }

		private void CollisionCheck(Player[] players)
        {
            for (int currentFireballIndex = 0; currentFireballIndex < fireballObjects.Count; currentFireballIndex++)
            {
                GameObject fireball = fireballObjects[currentFireballIndex];

                for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                {
                    Player currentPlayer = players[currentPlayerIndex];

                    if (Collision.IsColliding(currentPlayer, fireball))
					{
                        Console.WriteLine($"{currentPlayerIndex} is colliding with {currentFireballIndex}.");

                        placements.Add(new Placement()
                        {
                            PlayerIndex = currentPlayerIndex,
                            PlayerPlacement = Main.Instance.TotalPlayers - placements.Count,
                            PlayerText = ""
                        });

                        currentPlayer.Active = false;

                        if (placements.Count >= Main.Instance.TotalPlayers)
						{
                            Main.Instance.ChangeGameState(new EndScreen(placements, "CannonDodge"));

                            return;
						}
					}
                }
            }
        }

		public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            foreach (GameObject fireball in fireballObjects)
                fireball.Draw(spriteBatch);

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].Draw(spriteBatch);
        }
    }
}
