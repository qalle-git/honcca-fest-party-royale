using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.MainClasses
{
    public class Input
    {

        public enum KeySet
        {
            WASD,
            ARROWS,
            IJKL
        }

        public static Dictionary<KeySet, Keys[]> ActionKeys = new Dictionary<KeySet, Keys[]>()
        {
            {
                KeySet.WASD, new Keys[5]
                {
                    Keys.W,
                    Keys.A,
                    Keys.S,
                    Keys.D,
                    Keys.Space
                }
            },
            {
                KeySet.ARROWS, new Keys[5]
                {
                    Keys.Up,
                    Keys.Left,
                    Keys.Down,
                    Keys.Right,
                    Keys.Enter
                }
            },
            //{
            //    KeySet.IJKL, new Keys[5]
            //    {
            //        Keys.I,
            //        Keys.J,
            //        Keys.K,
            //        Keys.L
            //    }
            //}
        };

        public static Keys[] GetPressedKeys()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            return keyboardState.GetPressedKeys();
        }

        public static bool IsKeyDown(Keys key)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            return keyboardState.IsKeyDown(key);
        }
    }
}
