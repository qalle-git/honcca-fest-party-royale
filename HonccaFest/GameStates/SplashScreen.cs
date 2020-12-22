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
    class SplashScreen : Transition
    {
        private float rotation = 0f;

        public SplashScreen(GameState newGameState) : base(newGameState)
        {
            LevelName = "";
        }

        public override void Update(GameTime gameTime, Player[] players)
        {
            base.Update(gameTime, players);

            double loadingPercent = (gameTime.TotalGameTime.TotalMilliseconds - StartedTransition.TotalMilliseconds) / LoadingTimer.TotalMilliseconds * 100;

            if (loadingPercent < 83)
                rotation++;
        }
        
        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            Texture2D honccaLogo = Main.GraphicsHandler.GetSprite("HonccaLogo");

            spriteBatch.Draw(honccaLogo, new Vector2((Globals.ScreenSize.X / 2) - (honccaLogo.Width / 2), (Globals.ScreenSize.Y / 2) - (honccaLogo.Height / 2)), Color.White);

            spriteBatch.DrawString(Main.MainFont, "Honcca Fest: Party Royale", new Vector2(300, 350), Color.White, rotation, Vector2.Zero, rotation / 100, SpriteEffects.None, 1);
        }
    }
}
