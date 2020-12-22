// Player.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using HonccaFest.Files;
using HonccaFest.MainClasses;
using HonccaFest.Sound;
using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static HonccaFest.MainClasses.Input;

namespace HonccaFest
{
    public class Player : Animation
    {
        public readonly KeySet MovementSet;

        public bool MovementEnabled = true;

        public bool IsStunned = false;

        public TimeSpan TimeStunned = TimeSpan.Zero;
        public TimeSpan TimeStunnedCooldown;

        private readonly Animation stunObject;

        public Player(Texture2D texture, Vector2 position, KeySet _movementSet) : base(texture, position)
        {
            MovementSet = _movementSet;

            SetAnimationData(new Point(3, 4), new Point(0, 3), Animation.Direction.RIGHT);

            stunObject = new Animation(Main.GraphicsHandler.GetSprite("LoadingCircleSprite"), new Vector2(5, 5))
            {
                Active = true,
                CurrentState = State.ANIMATING
            };

            stunObject.SetAnimationData(new Point(12, 4), new Point(0, 12), Direction.RIGHT, 50, 1, true);
        }

        private TimeSpan honkCooldown = TimeSpan.FromMilliseconds(1000);
        private TimeSpan lastHonk = TimeSpan.Zero;

        public override void Update(GameTime gameTime, Tile[,][] map)
        {
            if (!Active)
                return;

            if (MovementEnabled && !ChangingTile)
                InputHandler(gameTime, map);

            StunHandler(gameTime, map);

            if (ChangingTile)
                CurrentState = State.ANIMATING;
            else
                CurrentState = State.IDLE;

            base.Update(gameTime, map);
        }

        private void StunHandler(GameTime gameTime, Tile[,][] map)
        {
            if (!IsStunned)
                return;

            if (TimeStunned == TimeSpan.Zero)
                TimeStunned = gameTime.TotalGameTime;

            stunObject.Update(gameTime, map);

            if (gameTime.TotalGameTime > TimeStunned + TimeStunnedCooldown + stunObject.AnimationCooldown + stunObject.AnimationCooldown)
            {
                IsStunned = false;

                TimeStunned = TimeSpan.Zero;

                stunObject.Active = false;

                MovementEnabled = true;
            }

            stunObject.CurrentPixelPosition = CurrentPixelPosition;
        }

        private void InputHandler(GameTime gameTime, Tile[,][] map)
        {
            if (IsUsingActionKey(ArcadeButton.Up))
            {
                CurrentFrame.Y = 3;

                Move(gameTime, new Vector2(CurrentPosition.X, CurrentPosition.Y - 1), map);
            }
            else if (IsUsingActionKey(ArcadeButton.Left))
            {
                CurrentFrame.Y = 1;

                Move(gameTime, new Vector2(CurrentPosition.X - 1, CurrentPosition.Y), map);
            }
            else if (IsUsingActionKey(ArcadeButton.Down))
            {
                CurrentFrame.Y = 0;

                Move(gameTime, new Vector2(CurrentPosition.X, CurrentPosition.Y + 1), map);
            }
            else if (IsUsingActionKey(ArcadeButton.Right))
            {
                CurrentFrame.Y = 2;

                Move(gameTime, new Vector2(CurrentPosition.X + 1, CurrentPosition.Y), map);
            }
            else if (JustPressedActionKey(ArcadeButton.Red))
            {
                if (gameTime.TotalGameTime > lastHonk + honkCooldown)
                {
                    AudioEffect sound = new AudioEffect("quack_sound");

                    sound.Play(0.1f, CurrentPosition);

                    lastHonk = gameTime.TotalGameTime;
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!Active)
                return;

            Rectangle drawRectangle = new Rectangle((int)CurrentPixelPosition.X, (int)CurrentPixelPosition.Y, Globals.TileSize.X * Multiplier, Globals.TileSize.Y * Multiplier);
            Rectangle sourceRectangle = new Rectangle(CurrentFrame.X * Globals.TileSize.X, CurrentFrame.Y * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y);

            sb.Draw(Texture, drawRectangle, sourceRectangle, CurrentColor, 0f, Vector2.Zero, CurrentDirection == Direction.LEFT ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            if (IsStunned)
                stunObject.Draw(sb);
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

            if (Collision.TilesHasCollision(tiles))
                return;

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

        /// <summary>
        /// Stun the player object for a certain amount of time.
        /// </summary>
        /// <param name="stunner">The Player who stunned this player</param>
        /// <param name="timeStunned">How long the player should be stunned.</param>
        public void GetStunned(Player stunner, float timeStunned = 3000)
        {
            if (!Active || IsStunned)
                return;

            stunObject.Active = true;
            stunObject.CurrentColor = Color.White;

            stunObject.AnimationSpeed = timeStunned / (stunObject.TotalFrames.X * stunObject.TotalFrames.Y);

            stunObject.AnimationCooldown = TimeSpan.FromMilliseconds(stunObject.AnimationSpeed);

            stunObject.CurrentFrame = new Point(0, 0);

            IsStunned = true;

            TimeStunnedCooldown = TimeSpan.FromMilliseconds(timeStunned);

            MovementEnabled = false;
        }
    }
}
