using HonccaFest.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.GameStates
{
    class Transition : GameState
    {
        private int loadingBarWidth = 0;

        private TimeSpan startedTransition = TimeSpan.Zero;
        private TimeSpan loadingTimer = TimeSpan.FromSeconds(4);

        // The GameState which we will change to after loadingTimer has been past.
        private readonly GameState loadingGameState;

        /// <summary>
        /// Tips to every GameMode, shows in the loadingscreen.
        /// </summary>
        private readonly Dictionary<string, string> tips = new Dictionary<string, string>()
        {
            {
                "DuckTag",
                "Tag, a game where you chase eachother and try to mark him, the one that's \"It\" the smallest amount of time wins.\n     Tag with your \"Green Button\"."
            },
            {
                "CannonDodge",
                "Cannons! Dodge the cannonballs thats firing against you, stun your enemies with \"Green Button\", last man standing wins!"
            }
        };

        public Transition(GameState newGameState) : base(newGameState.LevelName)
        {
            loadingGameState = newGameState;
        }

        public override void Initialize(ref Player[] players)
        {
            
        }

        public override void Update(GameTime gameTime, Player[] players)
        {
            if (startedTransition == TimeSpan.Zero)
                startedTransition = gameTime.TotalGameTime;

            double loadingPercent = (gameTime.TotalGameTime.TotalMilliseconds - startedTransition.TotalMilliseconds) / loadingTimer.TotalMilliseconds * 100;

            if (loadingPercent >= 108)
            {
                if (gameTime.TotalGameTime > startedTransition + loadingTimer + TimeSpan.FromSeconds(1))
                    Main.Instance.ChangeGameState(loadingGameState);
            } else
                loadingBarWidth = (int)loadingPercent * (Globals.ScreenSize.X / 100);
        }

        private const int loadingBarHeight = 20;

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            spriteBatch.Draw(Main.TranparentRectangle, new Rectangle(0, 0, Globals.ScreenSize.X, Globals.ScreenSize.Y), Color.White);

            spriteBatch.DrawString(Main.ScoreFont, LevelName, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(Main.MainFont, $"{tips[LevelName]}", new Vector2(30, 70), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0);

            Rectangle drawRectangle = new Rectangle(0, Globals.ScreenSize.Y - loadingBarHeight, loadingBarWidth, loadingBarHeight);

            spriteBatch.Draw(Main.OutlineRectangle, drawRectangle, Color.White);
        }
    }
}
