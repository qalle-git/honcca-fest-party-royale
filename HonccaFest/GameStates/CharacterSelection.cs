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
    class CharacterSelection : GameState
    {
        private readonly int startX = (Globals.ScreenSize.X / 5) - (Globals.TileSize.X / 4);
        private readonly int startY = Globals.ScreenSize.Y / 2 - Globals.TileSize.Y / 2;

        private int[] chosenCharacterRanges;

        /// <summary>
        /// Characters you can choose between inside the spritesheet.
        /// </summary>
        private Point[] characterRanges = new Point[]
        {
            new Point(0, 3),
            new Point(3, 6),
            new Point(6, 9),
            new Point(9, 12)
        };

        public CharacterSelection() : base("CharacterSelection")
        {
            chosenCharacterRanges = new int[4];
        }

        public override void Initialize(ref Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                currentPlayer.CurrentPixelPosition = new Vector2(startX + (startX * (currentPlayerIndex + 1 - 1)), startY);
            }
        }

        public override void Update(GameTime gameTime, Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                InputHandler(currentPlayerIndex, gameTime, players);
        }

        private void InputHandler(int currentPlayerIndex, GameTime gameTime, Player[] players)
        {
            Player currentPlayer = players[currentPlayerIndex];

            if (chosenCharacterRanges[currentPlayerIndex] < characterRanges.Length - 1)
            {
                if (currentPlayer.JustPressedActionKey(ArcadeButton.Right))
                {
                    ChangePlayerCharacter(currentPlayerIndex, 1, players);
                }
            }
            if (chosenCharacterRanges[currentPlayerIndex] > 0)
            {
                if (currentPlayer.JustPressedActionKey(ArcadeButton.Left))
                {
                    ChangePlayerCharacter(currentPlayerIndex, -1, players);
                }
            }
        }

        private void ChangePlayerCharacter(int currentPlayerIndex, int newIndex, Player[] players)
        {
            Player currentPlayer = players[currentPlayerIndex];

            chosenCharacterRanges[currentPlayerIndex] += newIndex;

            currentPlayer.SetAnimationData(currentPlayer.TotalFrames, characterRanges[chosenCharacterRanges[currentPlayerIndex]], Animation.Direction.RIGHT);

            Console.WriteLine($"{currentPlayerIndex} just changed to: {chosenCharacterRanges[currentPlayerIndex]}");
        }

        public override void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            base.Draw(spriteBatch, players);

            spriteBatch.Draw(Main.TranparentRectangle, new Rectangle(0, 0, Globals.ScreenSize.X, Globals.ScreenSize.Y), Color.White);

            foreach (Player player in players)
                player.Draw(spriteBatch);

            DrawPlacements(spriteBatch, players);
        }

        private void DrawPlacements(SpriteBatch spriteBatch, Player[] players)
        {
            for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
            {
                Player currentPlayer = players[currentPlayerIndex];

                string currentPlayerString = $"Player {currentPlayerIndex + 1}";

                Vector2 fontSize = Main.ScoreFont.MeasureString(currentPlayerString);

                spriteBatch.DrawString(Main.ScoreFont, currentPlayerString, new Vector2(startX + (startX * currentPlayerIndex) + Globals.TileSize.X / 2 - fontSize.X / 2, startY - 100), currentPlayer.Active ? Color.White : Color.DimGray);

                if (currentPlayer.Active)
                {
                    if (chosenCharacterRanges[currentPlayerIndex] < characterRanges.Length - 1)
                        spriteBatch.Draw(Main.CharacterSelectionArrow, new Rectangle(startX + (startX * currentPlayerIndex) + 50, startY + Globals.TileSize.Y / 4, Globals.TileSize.X / 2, Globals.TileSize.Y / 2), Color.White);
                    if (chosenCharacterRanges[currentPlayerIndex] > 0)
                        spriteBatch.Draw(Main.CharacterSelectionArrow, new Rectangle(startX + (startX * currentPlayerIndex) - (50 / 2 + (50 / 8)), startY + Globals.TileSize.Y / 4, Globals.TileSize.X / 2, Globals.TileSize.Y / 2), new Rectangle(0, 0, Main.CharacterSelectionArrow.Width, Main.CharacterSelectionArrow.Height), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);

                    spriteBatch.Draw(Main.OutlineRectangle, new Rectangle(startX + (startX * currentPlayerIndex), startY, Globals.TileSize.X, Globals.TileSize.Y), Color.White);
                }
            }

            string instructionJoystickString = $"READY";

            Vector2 instructionFontSize = Main.ScoreFont.MeasureString(instructionJoystickString);

            spriteBatch.DrawString(Main.ScoreFont, instructionJoystickString, new Vector2(Globals.ScreenSize.X / 2 - instructionFontSize.X / 2, startY + 100), Color.Green);

            int buttonSize = Globals.TileSize.X;

            spriteBatch.Draw(Main.JoystickButtons, new Rectangle((int)(Globals.ScreenSize.X / 2 - buttonSize / 2), startY + 150, buttonSize, buttonSize), new Rectangle(47, 39, 122, 122), Color.White);
        }
    }
}
