// Animation.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HonccaFest.MainClasses
{
    public class Animation : GameObject
    {
        public float AnimationSpeed;

        public Point TotalFrames;
        public Point CurrentFrame;

        public Point FrameRange;

        public int Multiplier;

        public bool FullAnimation;

        public State CurrentState = State.IDLE;
        public Direction CurrentDirection;

        /// <summary>
        /// The direction the animation will face, LEFT will flip the object horizontally.
        /// </summary>
        public enum Direction
        {
            LEFT,
            RIGHT
        }

        /// <summary>
        /// The state of the animation object, ANIMATING = the object will animate. IDLE = the object will not animate.
        /// </summary>
        public enum State
        {
            ANIMATING,
            IDLE
        }

		public override Rectangle GetRectangle()
		{
            return new Rectangle((int)CurrentPixelPosition.X, (int)CurrentPixelPosition.Y, Globals.TileSize.X, Globals.TileSize.Y);
        }

		public Animation(Texture2D texture, Vector2 position) : base(texture, position)
        {
        }

        /// <summary>
        /// Set the animation data to this object.
        /// </summary>
        /// <param name="_totalFrames">How many frames the total sprite is | X | Y |</param>
        /// <param name="_frameRange">The range this object will take use of in the X-axis</param>
        /// <param name="_direction">The direction the sprite should face | LEFT | RIGHT |</param>
        /// <param name="_animationSpeed">Animation speed.</param>
        /// <param name="_multiplier">Scale multiplier.</param>
        public void SetAnimationData(Point _totalFrames, Point _frameRange, Direction _direction, float _animationSpeed = 120f, int _multiplier = 1, bool _fullAnimation = false)
        {
            TotalFrames = _totalFrames;
            FrameRange = _frameRange;

            CurrentFrame.X = FrameRange.X;

            CurrentDirection = _direction;

            AnimationSpeed = _animationSpeed;

            Multiplier = _multiplier;
            FullAnimation = _fullAnimation;

            AnimationCooldown = TimeSpan.FromMilliseconds(AnimationSpeed);
        }

        public TimeSpan AnimationCooldown;
        public TimeSpan LastAnimation = TimeSpan.Zero;

        public override void Update(GameTime gameTime, Tile[,][] map)
        {
            if (!Active)
                return;

            if (CurrentState == State.ANIMATING)
                if (gameTime.TotalGameTime > LastAnimation + AnimationCooldown)
                {
                    int newFrame = CurrentFrame.X + 1;

                    if (newFrame < FrameRange.Y)
                        CurrentFrame.X++;
                    else
                    {
                        CurrentFrame.X = FrameRange.X;

                        if (FullAnimation)
                        {
                            int newFrameY = CurrentFrame.Y + 1;

                            if (newFrameY < TotalFrames.Y + 1)
                                CurrentFrame.Y++;
                            else
                                CurrentFrame.Y = 0;
                        }
                    }

                    LastAnimation = gameTime.TotalGameTime;
                }

            base.Update(gameTime, map);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!Active)
                return;

            Rectangle drawRectangle = new Rectangle((int)CurrentPixelPosition.X, (int)CurrentPixelPosition.Y, Globals.TileSize.X * Multiplier, Globals.TileSize.Y * Multiplier);
            Rectangle sourceRectangle = new Rectangle(CurrentFrame.X * Globals.TileSize.X, CurrentFrame.Y * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y);

            sb.Draw(Texture, drawRectangle, sourceRectangle, CurrentColor, 0f, Vector2.Zero, CurrentDirection == Direction.LEFT ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
    }
}
