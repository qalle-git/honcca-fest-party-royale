using HonccaFest.MainClasses;
using HonccaFest.Sound;
using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HonccaFest.MainClasses.Input;

namespace HonccaFest
{
    public class Player : Animation
    {
        public readonly KeySet MovementSet;

        public bool MovementEnabled = true;

        public Player(Texture2D texture, Vector2 position, KeySet _movementSet) : base(texture, position)
        {
            MovementSet = _movementSet;

            SetAnimationData(new Point(3, 4), new Point(0, 3), Animation.Direction.RIGHT);
        }

        private TimeSpan honkCooldown = TimeSpan.FromMilliseconds(1000);
        private TimeSpan lastHonk = TimeSpan.Zero;

        public override void Update(GameTime gameTime, Tile[,][] map)
        {
            if (!Active)
                return;

            if (MovementEnabled && !ChangingTile)
            {
                int playerIndex = (int)MovementSet;

                if (MonoArcade.GetKey(playerIndex, ArcadeButton.Up))
				{
                    CurrentFrame.Y = 3;

                    Move(gameTime, new Vector2(CurrentPosition.X, CurrentPosition.Y - 1), map);
                } else if (MonoArcade.GetKey(playerIndex, ArcadeButton.Left))
				{
                    CurrentFrame.Y = 1;

                    Move(gameTime, new Vector2(CurrentPosition.X - 1, CurrentPosition.Y), map);
                } else if (MonoArcade.GetKey(playerIndex, ArcadeButton.Down))
				{
                    CurrentFrame.Y = 0;

                    Move(gameTime, new Vector2(CurrentPosition.X, CurrentPosition.Y + 1), map);
                } else if (MonoArcade.GetKey(playerIndex, ArcadeButton.Right))
				{
                    CurrentFrame.Y = 2;

                    Move(gameTime, new Vector2(CurrentPosition.X + 1, CurrentPosition.Y), map);
                } else if (MonoArcade.GetKey(playerIndex, ArcadeButton.Blue))
				{
                    if (gameTime.TotalGameTime > lastHonk + honkCooldown)
                    {
                        AudioEffect sound = new AudioEffect("quack_sound");

                        sound.Play(0.1f, CurrentPixelPosition);

                        lastHonk = gameTime.TotalGameTime;
                    }
                }
            }

            base.Update(gameTime, map);
        }

        public override void Move(GameTime gameTime, Vector2 _newPosition, Tile[,][] _map)
        {
            if (_newPosition.X > Globals.GameSize.X - 1)
                return;
            else if (_newPosition.X < 0)
                return;
            else if (_newPosition.Y > Globals.GameSize.Y - 1)
                return;
            else if (_newPosition.Y < 0)
                return;

            Tile[] tiles = _map[(int)_newPosition.X, (int)_newPosition.Y];

            for (int currentTileIndex = 0; currentTileIndex < tiles.Length; currentTileIndex++)
			{
                Tile currentTile = tiles[currentTileIndex];

                if (currentTile.TileType == Tile.Type.COLLISION)
                    return;
            }

            base.Move(gameTime, _newPosition, _map);
        }

        public bool IsUsingActionKey(ArcadeButton currentKey)
        {
            int playerIndex = (int)MovementSet;

            return MonoArcade.GetKey(playerIndex, currentKey);
        }

        public bool JustPressedActionKey(ArcadeButton currentKey)
        {
            int playerIndex = (int)MovementSet;

            return MonoArcade.GetKeyDown(playerIndex, currentKey);
        }
    }
}
