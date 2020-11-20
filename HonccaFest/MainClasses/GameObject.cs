using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.MainClasses
{
    public class GameObject
    {
        public Texture2D Texture;

        public Vector2 CurrentPosition;
        public Vector2 CurrentPixelPosition;

        public bool ChangingTile;

        public bool Active = true;

        private float pixelPerMove = 2;
        public float PixelPerMove
        {
            get
            {
                return pixelPerMove;
            }
            set
            {
                if (value > 4)
                {
                    pixelPerMove = 4;

                    return;
                }

                pixelPerMove = value;
            }
        }

        public bool IsOutOfBounds
        {
            get
            {
                if (CurrentPosition.X > Globals.GameSize.X)
                    return true;
                else if (CurrentPosition.X < 0)
                    return true;
                else if (CurrentPosition.Y > Globals.GameSize.Y)
                    return true;
                else if (CurrentPosition.Y < 0)
                    return true;

                return false;
            }
        }

        public virtual Rectangle GetRectangle()
		{
            return new Rectangle((int)CurrentPixelPosition.X, (int)CurrentPixelPosition.Y, Texture.Width, Texture.Height);
		}

        public GameObject(Texture2D texture, Vector2 position)
        {
            Texture = texture;

            CurrentPosition = position;
            CurrentPixelPosition = new Vector2(CurrentPosition.X * Globals.TileSize.X, CurrentPosition.Y * Globals.TileSize.Y);
        }

        public virtual void Update(GameTime gameTime, Tile[,][] map)
        {
            if (!Active)
                return;

            if (ChangingTile)
            {
                if (CurrentPixelPosition.X < CurrentPosition.X * Globals.TileSize.X)
                    CurrentPixelPosition.X += pixelPerMove;
                if (CurrentPixelPosition.Y < CurrentPosition.Y * Globals.TileSize.Y)
                    CurrentPixelPosition.Y += pixelPerMove;
                if (CurrentPixelPosition.X > CurrentPosition.X * Globals.TileSize.X)
                    CurrentPixelPosition.X -= pixelPerMove;
                if (CurrentPixelPosition.Y > CurrentPosition.Y * Globals.TileSize.Y)
                    CurrentPixelPosition.Y -= pixelPerMove;

                if (CurrentPixelPosition == new Vector2(CurrentPosition.X * Globals.TileSize.X, CurrentPosition.Y * Globals.TileSize.Y))
                {
                    ChangingTile = false;
                }
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (!Active)
                return;

            Rectangle drawRectangle = new Rectangle((int)CurrentPixelPosition.X, (int)CurrentPixelPosition.Y, Globals.TileSize.X, Globals.TileSize.Y);

            sb.Draw(Texture, drawRectangle, Color.White);
        }

        private TimeSpan movementCooldown = TimeSpan.FromMilliseconds(150);
        private TimeSpan lastMovement = TimeSpan.Zero;

        public virtual void Move(GameTime gameTime, Vector2 _newPosition, Tile[,][] _map)
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
            CurrentPixelPosition = new Vector2(_newPosition.X * Globals.TileSize.X, _newPosition.Y * Globals.TileSize.Y);
        }
    }
}
