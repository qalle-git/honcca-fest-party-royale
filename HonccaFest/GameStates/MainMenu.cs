using HonccaFest.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static HonccaFest.MainClasses.Input;

namespace HonccaFest.GameStates
{
    class MainMenu : GameState
    {
        private int currentOption;
        private string[] menuOptions = new string[]
        {
            "START",
            "SETTINGS",
            "QUIT"
        };

        public MainMenu() : base("MainMenu")
        {
        }

        public override void Initialize(ref Player[] players)
        {
            players[0].MovementEnabled = false;
        }

        private TimeSpan optionChangeCooldown = TimeSpan.FromMilliseconds(200);
        private TimeSpan lastOptionChange = TimeSpan.Zero;

        public override void Update(GameTime gameTime, Player[] players)
        {
            Player playerOne = players[0];

            if (gameTime.TotalGameTime > lastOptionChange + optionChangeCooldown)
            {
                Keys[] movementKeys = ActionKeys[playerOne.MovementSet];

                for (int currentKeyIndex = 0; currentKeyIndex < movementKeys.Length; currentKeyIndex++)
                {
                    Keys currentKey = movementKeys[currentKeyIndex];

                    if (IsKeyDown(currentKey))
                    {
                        string menuOptionLabel = menuOptions[currentOption];

                        switch (currentKeyIndex)
                        {
                            case 0:
                                if (currentOption > 0)
                                    currentOption--;

                                break;
                            case 2:
                                if (currentOption < menuOptions.Length - 1)
                                    currentOption++;

                                break;
                            case 4:
                                // Enter on option
                                if (menuOptionLabel == "QUIT")
                                    Main.Instance.Exit();
                                else if (menuOptionLabel == "START")
                                    Main.Instance.ChangeGameState(new CannonDodge());

                                break;
                            default:
                                break;
                        }

                        lastOptionChange = gameTime.TotalGameTime;
                    }
                };
            }
        }

        private const int optionWidth = 300;
        private const int optionHeight = 75;

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            int startX = Globals.ScreenSize.X / 2 - optionWidth / 2;
            int startY = Globals.ScreenSize.Y / 2 - (optionHeight * menuOptions.Length / 2);

            for (int currentMenuIndex = 0; currentMenuIndex < menuOptions.Length; currentMenuIndex++)
            {
                string currentMenuLabel = menuOptions[currentMenuIndex];

                Vector2 fontSize = Main.MainFont.MeasureString(currentMenuLabel);

                spriteBatch.Draw(Main.OutlineRectangle, new Rectangle(startX, startY + (optionHeight * currentMenuIndex), optionWidth, optionHeight), currentOption == currentMenuIndex ? Color.Green : Color.Transparent);
                spriteBatch.DrawString(Main.MainFont, currentMenuLabel, new Vector2(startX + (optionWidth / 2 - fontSize.X / 2), startY + (optionHeight * currentMenuIndex) + (fontSize.Y / 2)), currentOption == currentMenuIndex ? Color.Green : Color.White);
            }
        }
    }
}
