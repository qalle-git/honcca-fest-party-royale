using HonccaFest.MainClasses;
using HonccaFest.Sound;
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
        }

        private TimeSpan honkCooldown = TimeSpan.FromMilliseconds(1000);
        private TimeSpan lastHonk = TimeSpan.Zero;

        public override void Update(GameTime gameTime)
        {
            if (MovementEnabled)
            {
                if (!ActionKeys.ContainsKey(MovementSet))
                    return;

                Keys[] movementKeys = ActionKeys[MovementSet];

                for (int currentKeyIndex = 0; currentKeyIndex < movementKeys.Length; currentKeyIndex++)
                {
                    Keys currentKey = movementKeys[currentKeyIndex];

                    if (IsKeyDown(currentKey))
                    {
                        switch (currentKeyIndex)
                        {
                            case 0:
                                CurrentFrame.Y = 3;

                                Move(gameTime, new Vector2(CurrentPosition.X, CurrentPosition.Y - 1));

                                break;
                            case 1:
                                CurrentFrame.Y = 1;

                                Move(gameTime, new Vector2(CurrentPosition.X - 1, CurrentPosition.Y));

                                break;
                            case 2:
                                CurrentFrame.Y = 0;

                                Move(gameTime, new Vector2(CurrentPosition.X, CurrentPosition.Y + 1));

                                break;
                            case 3:
                                CurrentFrame.Y = 2;

                                Move(gameTime, new Vector2(CurrentPosition.X + 1, CurrentPosition.Y));

                                break;
                            case 4:
                                if (gameTime.TotalGameTime > lastHonk + honkCooldown)
                                {
                                    AudioEffect sound = new AudioEffect("quack_sound");

                                    sound.Play(0.1f, CurrentPixelPosition);

                                    lastHonk = gameTime.TotalGameTime;
                                }

                                break;
                            default:
                                break;
                        }
                    }
                };

                base.Update(gameTime);
            }
        }

        public override void Move(GameTime gameTime, Vector2 _newPosition)
        {
            if (_newPosition.X >= Globals.GameSize.X)
                return;
            else if (_newPosition.X < 0)
                return;
            else if (_newPosition.Y >= Globals.GameSize.Y)
                return;
            else if (_newPosition.Y < 0)
                return;

            base.Move(gameTime, _newPosition);
        }

        public bool IsUsingActionKey(int keyIndex)
        {
            Keys[] movementKeys = ActionKeys[MovementSet];

            if (movementKeys.Length < keyIndex)
                return false;

            Keys currentKey = movementKeys[keyIndex];

            return IsKeyDown(currentKey);
        }
    }
}
