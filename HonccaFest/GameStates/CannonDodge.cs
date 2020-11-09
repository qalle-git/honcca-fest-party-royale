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
    class CannonDodge : GameState
    {
        private readonly List<GameObject> fireballObjects;

        public CannonDodge() : base("CannonDodge")
        {
            fireballObjects = new List<GameObject>();
        }

        public override void Initialize(ref Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].CurrentPosition = new Vector2(currentPlayerIndex * (players.Length), 18);
        }

        private TimeSpan fireballSpawnCooldown = TimeSpan.FromMilliseconds(1500);
        private TimeSpan lastFireballSpawn = TimeSpan.Zero;

        public override void Update(GameTime gameTime, Player[] players)
        {
            if (gameTime.TotalGameTime > fireballSpawnCooldown + lastFireballSpawn)
            {
                Vector2 randomSpawn = new Vector2(Main.RandomGenerator.Next(0, Map.GetLength(0)), 0);

                Animation fireballObject = new Animation(Main.FireballSprite, randomSpawn);

                fireballObject.SetAnimationData(new Point(7, 6), new Point(0, 6), Animation.Direction.RIGHT);

                fireballObject.CurrentFrame.Y = 6;

                fireballObjects.Add(fireballObject);

                lastFireballSpawn = gameTime.TotalGameTime;
            }

            for (int currentFireballIndex = 0; currentFireballIndex < fireballObjects.Count; currentFireballIndex++)
            {
                GameObject fireball = fireballObjects[currentFireballIndex];

                if (!fireball.IsOutOfBounds)
                {
                    fireball.Move(gameTime, new Vector2(fireball.CurrentPosition.X, fireball.CurrentPosition.Y + 1));

                    fireball.Update(gameTime);
                }
                else
                {
                    fireballObjects.RemoveAt(currentFireballIndex);

                    currentFireballIndex--;

                    Console.WriteLine("Deleted a fireball.");
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            foreach (GameObject fireball in fireballObjects)
                fireball.Draw(spriteBatch);
        }
    }
}
