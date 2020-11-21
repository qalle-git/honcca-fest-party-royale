using HonccaFest.Files;
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
	public class EndScreen : GameState
	{
		private readonly List<Placement> playerPlacements;

		private readonly int startX = (Globals.ScreenSize.X / 5) - (Globals.TileSize.X / 4);
		private readonly int startY = Globals.ScreenSize.Y / 2 - Globals.TileSize.Y / 2;

		private TimeSpan startedEndScreen = TimeSpan.Zero;
		private TimeSpan endScreenDuration = TimeSpan.FromSeconds(8);

		public EndScreen(List<Placement> placements, string levelName = "MainMenu") : base(levelName)
		{
			playerPlacements = placements;
		}

		public override void Initialize(ref Player[] players)
		{
			foreach (Placement placement in playerPlacements)
			{
				Player currentPlayer = players[placement.PlayerIndex];

				currentPlayer.CurrentPixelPosition = new Vector2(startX + (startX * (placement.PlayerPlacement - 1)), startY);

				currentPlayer.Active = MonoArcade.PlayerIsIngame(placement.PlayerIndex);
			}
		}

		public override void Update(GameTime gameTime, Player[] players)
		{
			if (startedEndScreen == TimeSpan.Zero)
				startedEndScreen = gameTime.TotalGameTime;

			if (gameTime.TotalGameTime > startedEndScreen + endScreenDuration)
			{
				Main.Instance.GamemodesPlayed++;

				GameState newGamemode = Main.Instance.GetRandomGameState(true);

				Main.Instance.ChangeGameState(new Transition(newGamemode));
			}
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
			foreach (Placement placement in playerPlacements)
			{
				bool isInGame = MonoArcade.PlayerIsIngame(placement.PlayerIndex);

				string currentPlayerString = $"Player {placement.PlayerIndex + 1}";

				Vector2 fontSize = Main.ScoreFont.MeasureString(currentPlayerString);

				spriteBatch.DrawString(Main.ScoreFont, currentPlayerString, new Vector2((startX + (startX * (placement.PlayerPlacement - 1))) + Globals.TileSize.X / 2 - fontSize.X / 2, startY - 100), isInGame ? Color.White : Color.DimGray);

				if (isInGame)
				{
					string currentPlacementString = $"#{placement.PlayerPlacement}";

					Vector2 placementFontSize = Main.ScoreFont.MeasureString(currentPlacementString);

					spriteBatch.DrawString(Main.ScoreFont, currentPlacementString, new Vector2(startX + (startX * (placement.PlayerPlacement - 1)) + Globals.TileSize.X / 2 - placementFontSize.X / 2, startY + 50), Color.White);

					string currentScoreString = $"{placement.PlayerText}";

					Vector2 scoreFontSize = Main.ScoreFont.MeasureString(currentScoreString);

					spriteBatch.DrawString(Main.ScoreFont, currentScoreString, new Vector2(startX + (startX * (placement.PlayerPlacement - 1)) + Globals.TileSize.X / 2 - scoreFontSize.X / 2, startY + 125), Color.White);
				
					spriteBatch.Draw(Main.OutlineRectangle, new Rectangle(startX + (startX * (placement.PlayerPlacement - 1)), startY, Globals.TileSize.X, Globals.TileSize.Y), Color.White);
				}
			}
		}
	}
}
