using HonccaFest.GameStates;
using HonccaFest.MainClasses;
using HonccaFest.MapCreator;
using HonccaFest.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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

        public static Texture2D PlayerOneSprite;

        public static Texture2D FireballSprite;

        public static SpriteFont MainFont;
        public static SpriteFont DebugFont;

        public static AudioHandler SoundHandler;

        public int TotalPlayers = 2;

        public const bool MapCreator = false;

        private Creator mapCreator;

        private GameState currentGameState;

        private Player[] players;

        public Main()
        {
            graphicsManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Globals.ScreenSize.X,
                PreferredBackBufferHeight = Globals.ScreenSize.Y
            };

            IsMouseVisible = true;

            graphicsManager.IsFullScreen = false;

            Window.Title = "Honcca Fest: Party Royale";

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            Instance = this;

            SoundHandler = new AudioHandler();

            if (MapCreator)
                mapCreator = new Creator();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TileSet = Content.Load<Texture2D>("Tiles/tileSet");
            OutlineRectangle = Content.Load<Texture2D>("Sprites/outlineRectangle");

            PlayerOneSprite = Content.Load<Texture2D>("SpriteSheets/playerSpritesheet");
            FireballSprite = Content.Load<Texture2D>("SpriteSheets/fireballSpritesheet");

            MainFont = Content.Load<SpriteFont>("Fonts/mainFont");

            if (MapCreator)
                DebugFont = Content.Load<SpriteFont>("Fonts/debugFont");
            else
            {
                players = new Player[TotalPlayers];

                for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                {
                    Player playerObject = new Player(PlayerOneSprite, new Vector2(currentPlayerIndex * (players.Length), 18), (KeySet)currentPlayerIndex);

                    playerObject.SetAnimationData(new Point(3, 4), new Point(currentPlayerIndex * 3, currentPlayerIndex * 3 + 3), Animation.Direction.RIGHT);

                    players[currentPlayerIndex] = playerObject;
                }

                currentGameState = new MainMenu();

                currentGameState.Initialize(ref players);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsKeyDown(Keys.Escape))
                Exit();

            if (MapCreator)
                mapCreator.Update(gameTime);
            else
            {
                currentGameState.Update(gameTime, players);

                for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                    players[currentPlayerIndex].Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (MapCreator)
                mapCreator.Draw(spriteBatch);
            else
            {
                currentGameState.Draw(spriteBatch, players);

                for (int currentPlayerIndex = 0; currentPlayerIndex < players.Length; currentPlayerIndex++)
                    players[currentPlayerIndex].Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ChangeGameState(GameState _newGameState)
        {
            currentGameState = _newGameState;

            currentGameState.Initialize(ref players);
        }
    }
}
