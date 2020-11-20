using HonccaFest.MainClasses;
using HonccaFest.Tiles;
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
            "MAP CREATOR",
            "SETTINGS",
            "QUIT"
        };

        public MainMenu() : base("Test")
        {
        }

        public override void Initialize(ref Player[] players)
        {
			for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
			{
                Player currentPlayer = players[currentPlayerIndex];

                currentPlayer.ForceMove(new Vector2(7, 5 + currentPlayerIndex));
			}
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
                                    Main.Instance.ChangeGameState(new CharacterSelection());
                                else if (menuOptionLabel == "MAP CREATOR")
                                    Main.Instance.ChangeGameState(new Creator());

                                break;
                            default:
                                break;
                        }

                        lastOptionChange = gameTime.TotalGameTime;
                    }
                };
            }

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].Update(gameTime, Map);
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

                spriteBatch.Draw(Main.OutlineRectangle, new Rectangle(startX, startY + (optionHeight * currentMenuIndex), optionWidth, optionHeight), currentOption == currentMenuIndex ? Color.Red : Color.Transparent);
                spriteBatch.DrawString(Main.MainFont, currentMenuLabel, new Vector2(startX + (optionWidth / 2 - fontSize.X / 2), startY + (optionHeight * currentMenuIndex) + (fontSize.Y / 2)), currentOption == currentMenuIndex ? Color.Red : Color.White);
            }

            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                players[currentPlayerIndex].Draw(spriteBatch);
        }
    }
}
