using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.MainClasses
{
    class GameObject
    {
        public Texture2D Texture;

        public Vector2 CurrentPosition;
        public Vector2 CurrentPixelPosition;

        public bool ChangingTile;

        public bool IsOutOfBounds
        {
            get
            {
                if (CurrentPosition.X > Main.GameSize.X)
                    return true;
                else if (CurrentPosition.X < 0)
                    return true;
                else if (CurrentPosition.Y > Main.GameSize.Y)
                    return true;
                else if (CurrentPosition.Y < 0)
                    return true;

                return false;
            }
        }

        private const float pixelPerMove = 2f;

        public GameObject(Texture2D texture, Vector2 position)
        {
            Texture = texture;

            CurrentPosition = position;
            CurrentPixelPosition = new Vector2(CurrentPosition.X * Main.TileSize.X, CurrentPosition.Y * Main.TileSize.Y);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (ChangingTile)
            {
                if (CurrentPixelPosition.X < CurrentPosition.X * Main.TileSize.X)
                    CurrentPixelPosition.X += pixelPerMove;
                if (CurrentPixelPosition.Y < CurrentPosition.Y * Main.TileSize.Y)
                    CurrentPixelPosition.Y += pixelPerMove;
                if (CurrentPixelPosition.X > CurrentPosition.X * Main.TileSize.X)
                    CurrentPixelPosition.X -= pixelPerMove;
                if (CurrentPixelPosition.Y > CurrentPosition.Y * Main.TileSize.Y)
                    CurrentPixelPosition.Y -= pixelPerMove;

                if (CurrentPixelPosition == new Vector2(CurrentPosition.X * Main.TileSize.X, CurrentPosition.Y * Main.TileSize.Y))
                {
                    ChangingTile = false;

                    Console.WriteLine("Done animation.");
                }
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            Rectangle drawRectangle = new Rectangle((int)CurrentPixelPosition.X, (int)CurrentPixelPosition.Y, Main.TileSize.X, Main.TileSize.Y);

            sb.Draw(Texture, drawRectangle, Color.White);
        }

        private TimeSpan movementCooldown = TimeSpan.FromMilliseconds(150);
        private TimeSpan lastMovement = TimeSpan.Zero;

        public virtual void Move(GameTime gameTime, Vector2 _newPosition)
        {
            if (gameTime.TotalGameTime > + lastMovement + movementCooldown && !ChangingTile)
            {
                CurrentPosition = _newPosition;

                lastMovement = gameTime.TotalGameTime;

                ChangingTile = true;
            }
        }

        public virtual void ForceMove(Vector2 _newPosition)
        {
            CurrentPosition = _newPosition;
            CurrentPixelPosition = new Vector2(_newPosition.X * Main.TileSize.X, _newPosition.Y * Main.TileSize.Y);
        }
    }
}
