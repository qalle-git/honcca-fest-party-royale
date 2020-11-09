using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.MainClasses
{
    class Input
    {

        public enum KeySet
        {
            WASD,
            ARROWS,
            IJKL
        }

        public static Dictionary<KeySet, Keys[]> MovementKeys = new Dictionary<KeySet, Keys[]>()
        {
            {
                KeySet.WASD, new Keys[4]
                {
                    Keys.W,
                    Keys.A,
                    Keys.S,
                    Keys.D
                }
            },
            {
                KeySet.ARROWS, new Keys[4]
                {
                    Keys.Up,
                    Keys.Left,
                    Keys.Down,
                    Keys.Right
                }
            },
            {
                KeySet.IJKL, new Keys[4]
                {
                    Keys.I,
                    Keys.J,
                    Keys.K,
                    Keys.L
                }
            }
        };

        public static Keys[] GetPressedKeys()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            return keyboardState.GetPressedKeys();
        }
    }
}
