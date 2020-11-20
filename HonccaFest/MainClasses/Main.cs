using HonccaFest.Files;
using HonccaFest.GameStates;
using HonccaFest.MainClasses;
using HonccaFest.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static HonccaFest.MainClasses.Input;

namespace HonccaFest
{
    public class Main : Game
    {
        public static Main Instance;

        private GraphicsDeviceManager graphicsManager;
        private SpriteBatch spriteBatch;

        public static Texture2D TileSet;
        public static Texture2D OutlineRectangle;
        public static Texture2D TranparentRectangle;
        public static Texture2D CharacterSelectionArrow;
        public static Texture2D JoystickButtons;

        public static Texture2D PlayerOneSprite;

        public static Texture2D FireballSprite;

        public static SpriteFont MainFont;
        public static SpriteFont ScoreFont;
        public static SpriteFont DebugFont;

        public static AudioHandler SoundHandler;

        public int TotalPlayers = 3;
        public const int MaxPlayers = 4;

        public int GamemodesPlayed = 0;

        public const bool MapCreator = true;

        public GameState CurrentGameState;

        private Player[] players;

        public Main()
        {
            graphicsManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Globals.ScreenSize.X,
                PreferredBackBufferHeight = Globals.ScreenSize.Y,

                IsFullScreen = false
            };

            IsMouseVisible = true;

            Window.Title = "Honcca Fest: Party Royale";

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            Instance = this;

            SoundHandler = new AudioHandler();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TileSet = Content.Load<Texture2D>("Tiles/tileSet");
            OutlineRectangle = Content.Load<Texture2D>("Sprites/outlineRectangle");
            TranparentRectangle = Content.Load<Texture2D>("Sprites/transparentRectangle");
            CharacterSelectionArrow = Content.Load<Texture2D>("Sprites/characterSelectionArrow");

            JoystickButtons = Content.Load<Texture2D>("Sprites/joystick");

            PlayerOneSprite = Content.Load<Texture2D>("SpriteSheets/playerSpritesheet");
            FireballSprite = Content.Load<Texture2D>("SpriteSheets/fireballSpritesheet");

            MainFont = Content.Load<SpriteFont>("Fonts/mainFont");
            ScoreFont = Content.Load<SpriteFont>("Fonts/scoreFont");

            DebugFont = Content.Load<SpriteFont>("Fonts/debugFont");

            players = new Player[MaxPlayers];

            for (int currentPlayerIndex = 0; currentPlayerIndex < MaxPlayers; currentPlayerIndex++)
            {
                Player playerObject = new Player(PlayerOneSprite, new Vector2(currentPlayerIndex * (players.Length), 18), (KeySet)currentPlayerIndex);

                if (currentPlayerIndex > TotalPlayers - 1)
                    playerObject.Active = false;

                players[currentPlayerIndex] = playerObject;
            }

            //List<Placement> fakePlacements = new List<Placement>()
            //{
            //	new Placement()
            //	{
            //		PlayerIndex = 1,
            //		PlayerPlacement = 1,
            //		PlayerText = "15s"
            //	},
            //	new Placement()
            //	{
            //		PlayerIndex = 0,
            //		PlayerPlacement = 2,
            //		PlayerText = "29s"
            //	},
            //	new Placement()
            //	{
            //		PlayerIndex = 2,
            //		PlayerPlacement = 3,
            //		PlayerText = "31s"
            //	},
            //	new Placement()
            //	{
            //		PlayerIndex = 3,
            //		PlayerPlacement = 4,
            //		PlayerText = "5s"
            //	},
            //};

            //CurrentGameState = new EndScreen(fakePlacements, "DuckTag");

            CurrentGameState = new MainMenu();

            CurrentGameState.Initialize(ref players);
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsKeyDown(Keys.Escape))
                Exit();

            CurrentGameState.Update(gameTime, players);

            if (GamemodesPlayed > Globals.MaxGamemodes)
                Exit();
            else if (IsAfk(gameTime))
                Exit();

            base.Update(gameTime);
        }

		protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            CurrentGameState.Draw(spriteBatch, players);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ChangeGameState(GameState _newGameState)
        {
            CurrentGameState = _newGameState;

            CurrentGameState.Initialize(ref players);
        }

        /// <summary>
        /// This will get a random game state.
        /// </summary>
        /// <param name="wantGamemode">If you only want to receive gamemodes.</param>
        /// <returns></returns>
        public GameState GetRandomGameState(bool wantGamemode = true)
		{
            List<GameState> gameModes = new List<GameState>()
            {
                new CannonDodge(),
                new DuckTag()
            };

            if (wantGamemode)
			{
                int randomGamemodeIndex = Globals.RandomGenerator.Next(0, gameModes.Count);

                return gameModes[randomGamemodeIndex];
			}

            return new MainMenu();
		}

        private TimeSpan lastPressed = TimeSpan.Zero;
        private TimeSpan afkTimer = TimeSpan.FromMinutes(2);

        /// <summary>
        /// Checks if the game is afk, no buttons pressed.
        /// </summary>
        /// <returns>Returns true if nothing is pressed within 1 minute.</returns>
        private bool IsAfk(GameTime gameTime)
        {
            int keysPressed = Input.GetPressedKeys().Length;

            if (keysPressed > 0)
			{
                lastPressed = gameTime.TotalGameTime;

                return false;
			}

            if (gameTime.TotalGameTime > lastPressed + afkTimer)
                return true;

            return false;
        }
    }
}
