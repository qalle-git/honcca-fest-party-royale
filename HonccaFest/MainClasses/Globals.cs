// Globals.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using Microsoft.Xna.Framework;
using System;

namespace HonccaFest.MainClasses
{
    class Globals
    {
        // This will activate certain debug methods and functions. Only for debugging.
        public static bool DebugMode = false;

        public static GraphicsDeviceManager GDManager;

        public static Point ScreenSize = new Point(1280, 720);

        public static Point TileSize = new Point(40, 40);
        public static Point GameSize = new Point(32, 18);

        public static int MaxGameModes = 10;

        public static int MaxPlayers = 4;

        public static Random RandomGenerator = new Random();

        /// <summary>
        /// This will send out a message to the console, only if debug mode is activated.
        /// </summary>
        /// <param name="_message">The message you want to send out.</param>
        public static void DebugPrint(string _message)
        {
            if (!DebugMode)
                return;

            Console.WriteLine(_message);
        }
    }
}
