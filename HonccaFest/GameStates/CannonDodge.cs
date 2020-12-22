// CannonDodge.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using HonccaFest.Files;
using HonccaFest.MainClasses;
using HonccaFest.Sound;
using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace HonccaFest.GameStates
{
    class CannonDodge : GameState
    {
        private List<GameObject> fireballObjects;
        private List<Cannon> cannonObjects;

        private float fireballSpawnCooldown = 400;

        // The final placement for each player.
        private readonly List<Placement> placements = new List<Placement>();

        // Each players spawnpoint.
        private readonly Vector2[] spawnPoints = new Vector2[]
        {
            new Vector2(5, 15),
            new Vector2(11, 17),
            new Vector2(19, 16),
            new Vector2(30, 15)
        };

        public CannonDodge() : base("CannonDodge")
        {
        }

        /// <summary>
        /// The regular initialize method, just initalizes everything needed.
        /// </summary>
        /// <param name="players">The players array.</param>
        public override void Initialize(ref Player[] players)
        {
            fireballObjects = new List<GameObject>();
            cannonObjects = new List<Cannon>();

            InitializePlayers(ref players);

            SpawnCannons();

            Main.MusicHandler.Play("wandering_maze");
        }

        /// <summary>
        /// Will initialize starting position to each player.
        /// </summary>
        /// <param name="players"></param>
        private void InitializePlayers(ref Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                currentPlayer.ForceMove(spawnPoints[currentPlayerIndex]);

                currentPlayer.MovementEnabled = true;

                // Put players who aren't active last in placement.
                if (!currentPlayer.Active)
                {
                    placements.Add(new Placement()
                    {
                        PlayerIndex = currentPlayerIndex,
                        PlayerPlacement = players.Length - placements.Count,
                        PlayerText = ""
                    });
                }
            }
        }

        private TimeSpan lastFireballSpawn = TimeSpan.Zero;

        public override void Update(GameTime gameTime, Player[] players)
        {
            StunHandler(gameTime, players);

            FireballSpawner(gameTime);
            FireballHandler(gameTime);

            CannonHandler(gameTime);

            CollisionCheck(players);

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].Update(gameTime, Map);
        }

        /// <summary>
        /// This method moves the cannons in the specified direction, deletes them when they're out of bounds.
        /// </summary>
        /// <param name="gameTime">GameTime object</param>
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

        /// <summary>
        /// This method takes care of cannonball spawning.
        /// </summary>
        /// <param name="gameTime">GameTime object</param>
		private void FireballSpawner(GameTime gameTime)
		{
            if (gameTime.TotalGameTime > TimeSpan.FromMilliseconds(fireballSpawnCooldown) + lastFireballSpawn)
            {
                int randomSpawn = Globals.RandomGenerator.Next(0, Map.GetLength(0));

                Cannon randomCannon = cannonObjects[randomSpawn];

                randomCannon.Shoot(ref fireballObjects);

                lastFireballSpawn = gameTime.TotalGameTime;

                fireballSpawnCooldown -= 1f;
            }

            fireballSpawnCooldown = MathHelper.Clamp(fireballSpawnCooldown, 50, 1000);
        }

        /// <summary>
        /// Checks if there is any player colliding with a cannonball
        /// </summary>
        /// <param name="players">The players array</param>
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
                        Globals.DebugPrint($"{currentPlayerIndex} is now colliding with {currentFireballIndex}.");

                        placements.Add(new Placement()
                        {
                            PlayerIndex = currentPlayerIndex,
                            PlayerPlacement = players.Length - placements.Count,
                            PlayerText = ""
                        });

                        currentPlayer.Active = false;

                        if (placements.Count >= players.Length)
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
            foreach (Cannon cannon in cannonObjects)
                cannon.Draw(spriteBatch);

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].Draw(spriteBatch);
        }

        private void CannonHandler(GameTime gameTime)
        {
            foreach (Cannon cannon in cannonObjects)
                cannon.Update(gameTime, Map);
        }

        /// <summary>
        /// Spawn the cannons uptop.
        /// </summary>
        private void SpawnCannons()
        {
            for (int currentX = 0; currentX < Globals.GameSize.X; currentX++)
            {
                Cannon newCannonObject = new Cannon(new Vector2(currentX, 0));

                cannonObjects.Add(newCannonObject);
            }
        }
    }

    public class Cannon : Animation
    {
        public Cannon(Vector2 position) : base(Main.GraphicsHandler.GetSprite("CannonSprite"), position)
        {
            SetAnimationData(new Point(1, 0), new Point(0, 2), Direction.LEFT, 400);
        }

        public override void Update(GameTime gameTime, Tile[,][] map)
        {
            if (CurrentState == State.ANIMATING)
            {
                if (gameTime.TotalGameTime > LastAnimation + AnimationCooldown)
                {
                    int newFrame = CurrentFrame.X + 1;

                    if (newFrame >= FrameRange.Y)
                    {
                        CurrentState = State.IDLE;

                        CurrentFrame.X = 0;

                        AudioEffect cannonSound = new AudioEffect("cannon_sound");

                        cannonSound.Play(0.05f, CurrentPosition);
                    }
                }
            }

            base.Update(gameTime, map);
        }

        public void Shoot(ref List<GameObject> fireballObjects)
        {
            CurrentState = State.ANIMATING;

            GameObject fireballObject = new GameObject(Main.GraphicsHandler.GetSprite("CannonBallSprite"), new Vector2(CurrentPosition.X, -1))
            {
                PixelPerMove = 4
            };

            fireballObjects.Add(fireballObject);
        }
    }
}
