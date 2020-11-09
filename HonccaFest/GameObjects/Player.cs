using HonccaFest.MainClasses;
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
    class Player : Animation
    {
        private KeySet movementSet;

        public Player(Texture2D texture, Vector2 position, KeySet _movementSet) : base(texture, position)
        {
            movementSet = _movementSet;
        }

        public override void Update(GameTime gameTime)
        {
            if (!ActionKeys.ContainsKey(movementSet))
                return;

            Keys[] movementKeys = ActionKeys[movementSet];

            for (int currentKeyIndex = 0; currentKeyIndex < movementKeys.Length; currentKeyIndex++)
            {
                Keys currentKey = movementKeys[currentKeyIndex];

                if (Keyboard.GetState().IsKeyDown(currentKey))
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
                        default:
                            break;
                    }
                }
            };

            base.Update(gameTime);
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
            Keys[] movementKeys = ActionKeys[movementSet];

            if (movementKeys.Length < keyIndex)
                return false;

            Keys currentKey = movementKeys[keyIndex];

            if (Keyboard.GetState().IsKeyDown(currentKey))
                return true;

            return false;
        }
    }
}
