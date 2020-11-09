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

        public State CurrentState = State.IDLE;
        public Direction CurrentDirection;

        public enum Direction
        {
            LEFT,
            RIGHT
        }

        public enum State
        {
            MOVING,
            IDLE
        }

        public Animation(Texture2D texture, Vector2 position) : base(texture, position)
        {
            Console.WriteLine($"Created animation class.");
        }

        public void SetAnimationData(Point _totalFrames, Point _frameRange, Direction _direction, float _animationSpeed = 120f)
        {
            TotalFrames = _totalFrames;
            FrameRange = _frameRange;

            CurrentFrame.X = FrameRange.X;

            CurrentDirection = _direction;

            AnimationSpeed = _animationSpeed;

            animationCooldown = TimeSpan.FromMilliseconds(AnimationSpeed);
        }

        private TimeSpan animationCooldown;
        private TimeSpan lastAnimation = TimeSpan.Zero;

        public override void Update(GameTime gameTime)
        {
            if (CurrentState == State.MOVING)
                if (gameTime.TotalGameTime > lastAnimation + animationCooldown)
                {
                    int newFrame = CurrentFrame.X + 1;

                    if (newFrame < FrameRange.Y)
                        CurrentFrame.X++;
                    else
                        CurrentFrame.X = FrameRange.X;

                    lastAnimation = gameTime.TotalGameTime;
                }

            if (ChangingTile)
                CurrentState = State.MOVING;
            else
                CurrentState = State.IDLE;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            Rectangle drawRectangle = new Rectangle((int)CurrentPixelPosition.X, (int)CurrentPixelPosition.Y, Globals.TileSize.X, Globals.TileSize.Y);
            Rectangle sourceRectangle = new Rectangle(CurrentFrame.X * Globals.TileSize.X, CurrentFrame.Y * Globals.TileSize.Y, Globals.TileSize.X, Globals.TileSize.Y);

            sb.Draw(Texture, drawRectangle, sourceRectangle, Color.White, 0f, Vector2.Zero, CurrentDirection == Direction.LEFT ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
    }
}
