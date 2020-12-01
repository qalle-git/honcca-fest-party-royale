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
        public static Texture2D TaggerArrow;
        public static Texture2D CheckMark;

        public static Texture2D JoystickButtons;

        public static Texture2D PlayerOneSprite;

        public static Texture2D FireballSprite;

        public static SpriteFont MainFont;
        public static SpriteFont ScoreFont;
        public static SpriteFont DebugFont;

        public static AudioHandler SoundHandler;

        public int TotalPlayers = 0;

        public int GamemodesPlayed = 0;

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

            SoundHandler = new AudioHandler();
        }

        protected override void LoadContent()
        {
            Instance = this;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Tile SpriteSheet
            TileSet = Content.Load<Texture2D>("Tiles/tileSet");

            // Sprites
            OutlineRectangle = Content.Load<Texture2D>("Sprites/outlineRectangle");
            TranparentRectangle = Content.Load<Texture2D>("Sprites/transparentRectangle");
            TaggerArrow = Content.Load<Texture2D>("Sprites/taggerPointer");
            CharacterSelectionArrow = Content.Load<Texture2D>("Sprites/characterSelectionArrow");
            CheckMark = Content.Load<Texture2D>("Sprites/checkmark");
            JoystickButtons = Content.Load<Texture2D>("Sprites/joystick");

            // SpriteSheets
            PlayerOneSprite = Content.Load<Texture2D>("SpriteSheets/playerSpritesheet");
            FireballSprite = Content.Load<Texture2D>("SpriteSheets/fireballSpritesheet");

            // Fonts
            MainFont = Content.Load<SpriteFont>("Fonts/mainFont");
            ScoreFont = Content.Load<SpriteFont>("Fonts/scoreFont");

            DebugFont = Content.Load<SpriteFont>("Fonts/debugFont");

            players = new Player[Globals.MaxPlayers];

            if (Globals.DebugMode)
                MonoArcade.ActivateDebug(false, true, true, false);

            for (int currentPlayerIndex = 0; currentPlayerIndex < Globals.MaxPlayers; currentPlayerIndex++)
            {
                bool isInGame = MonoArcade.PlayerIsIngame(currentPlayerIndex);

                Player playerObject = new Player(PlayerOneSprite, Vector2.Zero, (KeySet)currentPlayerIndex)
                {
                    Active = isInGame
                };

                if (isInGame)
                    TotalPlayers++;

                players[currentPlayerIndex] = playerObject;
            }

            if (Globals.DebugMode)
                ChangeGameState(new MainMenu());
            else
                ChangeGameState(new CharacterSelection());
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsKeyDown(Keys.Escape))
                Exit();

            CurrentGameState.Update(gameTime, players);

            if (GamemodesPlayed > Globals.MaxGameModes)
                Exit();
            else if (IsAfk(gameTime))
                Exit();

            base.Update(gameTime);
        }

		protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // Draw current gameState, map, players, ui etc.
            CurrentGameState.Draw(spriteBatch, players);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Change the current GameState to a new specified one.
        /// </summary>
        /// <param name="_newGameState">The new gamestate object.</param>
        public void ChangeGameState(GameState _newGameState)
        {
            lastGameState = _newGameState.LevelName;

            CurrentGameState = _newGameState;

            CurrentGameState.Initialize(ref players);
        }

        private string lastGameState;

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
                int randomGameState = Globals.RandomGenerator.Next(0, gameModes.Count);

                Console.WriteLine($"Checking {gameModes[randomGameState].LevelName} with {lastGameState}");

                if (gameModes[randomGameState].LevelName == lastGameState)
                    return GetRandomGameState(true);

                return gameModes[randomGameState];
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
