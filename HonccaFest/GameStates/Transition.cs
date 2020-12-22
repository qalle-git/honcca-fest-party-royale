// Transition.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using HonccaFest.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace HonccaFest.GameStates
{
    class Transition : GameState
    {
        private int loadingBarWidth = 0;

        public Animation Duck;

        public TimeSpan StartedTransition = TimeSpan.Zero;
        public TimeSpan LoadingTimer = TimeSpan.FromSeconds(2);

        private const int loadingBarHeight = 20;

        // The GameState which we will change to after loadingTimer has been past.
        private readonly GameState loadingGameState;

        /// <summary>
        /// Tips to every GameMode, shows in the loadingscreen.
        /// </summary>
        private readonly Dictionary<string, string> tips = new Dictionary<string, string>()
        {
            {
                "DuckTag",
                "Run away from the tagger while the tagger tries to tag the other players! Least amount of time as tagger wins!\n     Tag with \"Green Button\"."
            },
            {
                "DuckOut",
                "Boxing, Press the Yellow button to punch the other people, Last one standing or the one with the most lives at the end wins!"
            },
            {
                "CannonDodge",
                "Cannons! Dodge the cannonballs thats firing against you, stun your enemies with the \"Yellow Button\", last man standing wins!"
            },
            {
                "QuackCash",
                "Press \"Green Button\" when standing on coins to collect them. Richest player when the time is up wins!"
            },
            {
                "DuckyRoad",
                "Collect as many sacks as you can. The player with the most sacks collected wins!"
            },
            {
                "Quackory",
                "Stand on the correct fruit to survive, otherwise you're a fruit salad."
            },
            {
                "UltimateDuckRun",
                "Run for your life and finish as fast as you can! Fastest player is the ultimate duck!"
            },
            {
                "MazeOut",
                "Find your way out of the maze before the other players! Fastest player wins!"
            }
        };

        public Transition(GameState newGameState) : base(newGameState.LevelName)
        {
            loadingGameState = newGameState;
        }

        public override void Initialize(ref Player[] players)
        {
            Main.MusicHandler.Play("loading", false);

            Duck = new Animation(Main.GraphicsHandler.GetSprite("PlayerOneSprite"), new Vector2(0, Globals.GameSize.Y - 1))
            {
                CurrentState = Animation.State.ANIMATING,
                PixelPerMove = 8,
                CurrentFrame = new Point(0, 2),
                MovementCooldown = TimeSpan.FromMilliseconds(0)
            };
            Duck.SetAnimationData(new Point(3, 4), new Point(0, 3), Animation.Direction.RIGHT);
        }

        public override void Update(GameTime gameTime, Player[] players)
        {
            if (StartedTransition == TimeSpan.Zero)
                StartedTransition = gameTime.TotalGameTime;

            double loadingPercent = (gameTime.TotalGameTime.TotalMilliseconds - StartedTransition.TotalMilliseconds) / LoadingTimer.TotalMilliseconds * 100;

            if (loadingPercent >= 108)
            {
                if (gameTime.TotalGameTime > StartedTransition + LoadingTimer + TimeSpan.FromSeconds(1))
                    Main.Instance.ChangeGameState(loadingGameState);
            } else
                loadingBarWidth = (int)loadingPercent * (Globals.ScreenSize.X / 100);

            Rectangle duckRectangle = Duck.GetRectangle();

            Duck.CurrentPixelPosition = new Vector2(loadingBarWidth - duckRectangle.Width, Globals.ScreenSize.Y - duckRectangle.Height - loadingBarHeight);

            Duck.Update(gameTime, Map);
        }


        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            spriteBatch.Draw(Main.GraphicsHandler.GetSprite("TransparentRectangle"), new Rectangle(0, 0, Globals.ScreenSize.X, Globals.ScreenSize.Y), Color.White);

            spriteBatch.DrawString(Main.ScoreFont, LevelName, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(Main.MainFont, $"{(tips.ContainsKey(LevelName) ? tips[LevelName] : "")}", new Vector2(30, 70), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0);

            Rectangle drawRectangle = new Rectangle(0, Globals.ScreenSize.Y - loadingBarHeight, loadingBarWidth, loadingBarHeight);

            spriteBatch.Draw(Main.GraphicsHandler.GetSprite("OutlineRectangle"), drawRectangle, Color.White);

            Duck.Draw(spriteBatch);
        }
    }
}
