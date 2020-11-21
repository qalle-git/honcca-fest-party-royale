using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.MainClasses
{
    class Globals
    {
        public static Point ScreenSize = new Point(1280, 720);

        public static Point TileSize = new Point(40, 40);
        public static Point GameSize = new Point(32, 18);

        public static int MaxGamemodes = 5;

        public static int MaxPlayers = 4;

        public static Random RandomGenerator = new Random();
    }
}
