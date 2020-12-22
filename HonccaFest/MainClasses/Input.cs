// Input.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace HonccaFest.MainClasses
{
    public class Input
    {
        #region KeySets
        public enum KeySet
        {
            WASD,
            ARROWS,
            IJKL,
            TFGH
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
			{
				KeySet.IJKL, new Keys[5]
				{
					Keys.I,
					Keys.J,
					Keys.K,
					Keys.L,
                    Keys.P
				}
			},
            {
                KeySet.TFGH, new Keys[5]
                {
                    Keys.T,
                    Keys.F,
                    Keys.G,
                    Keys.H,
                    Keys.U
                }
            }
		};
        #endregion

        /// <summary>
        /// The current keys that are pressed.
        /// </summary>
        /// <returns>An array with all pressed keys.</returns>
        public static Keys[] GetPressedKeys()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            return keyboardState.GetPressedKeys();
        }

        /// <summary>
        /// Checks if a certain key is currently being pressed.
        /// </summary>
        /// <param name="key">The key you wan't to check.</param>
        /// <returns>True if the button is pressed, or false if it's not.</returns>
        public static bool IsKeyDown(Keys key)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            return keyboardState.IsKeyDown(key);
        }
    }
}
