// GameObject.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HonccaFest.MainClasses
{
    public class GameObject
    {
        public Texture2D Texture;

        public Vector2 CurrentPosition;
        public Vector2 CurrentPixelPosition;

        public SpriteEffects CurrentSpriteEffect = SpriteEffects.None;

        public Color CurrentColor = Color.White;

        public float Rotation;

        public bool ChangingTile;

        public bool Active = true;

        private float pixelPerMove = 2;

        /// <summary>
        /// This is the float that specifies how far the object will animate each frame between tiles.
        /// </summary>
        public float PixelPerMove
        {
            get
            {
                return pixelPerMove;
            }
            set
            {
                if (Globals.TileSize.X / value % 2 != 0)
                {
                    pixelPerMove = 4;

                    return;
                }

                pixelPerMove = value;
            }
        }

        /// <summary>
        /// Checks if the object is outside the map.
        /// </summary>
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
                    ChangingTile = false;
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (!Active)
                return;

            Rectangle drawRectangle = new Rectangle((int)CurrentPixelPosition.X, (int)CurrentPixelPosition.Y, Globals.TileSize.X, Globals.TileSize.Y);

            sb.Draw(Texture, drawRectangle, new Rectangle(0, 0, Texture.Width, Texture.Height), CurrentColor, Rotation, Vector2.Zero, CurrentSpriteEffect, 0);
        }

        public TimeSpan MovementCooldown = TimeSpan.FromMilliseconds(150);
        private TimeSpan lastMovement = TimeSpan.Zero;

        /// <summary>
        /// Move the object to a certain animation, this will animate the object to that position.
        /// </summary>
        /// <param name="gameTime">The gameTime.</param>
        /// <param name="_newPosition">The position the player will animate to.</param>
        /// <param name="_map">The map, this is for collision checks.</param>
        public virtual void Move(GameTime gameTime, Vector2 _newPosition, Tile[,][] _map)
        {
            if (gameTime.TotalGameTime > + lastMovement + MovementCooldown && !ChangingTile)
            {
                CurrentPosition = _newPosition;

                lastMovement = gameTime.TotalGameTime;

                ChangingTile = true;
            }
        }

        /// <summary>
        /// This will teleport the player to the specified position.
        /// </summary>
        /// <param name="_newPosition">The position the player will teleport to.</param>
        public virtual void ForceMove(Vector2 _newPosition)
        {
            CurrentPosition = _newPosition;
            CurrentPixelPosition = new Vector2(_newPosition.X * Globals.TileSize.X, _newPosition.Y * Globals.TileSize.Y);
        }
    }
}
