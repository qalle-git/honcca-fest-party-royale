using HonccaFest.MapCreator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HonccaFest
{
    public class Main : Game
    {
        private GraphicsDeviceManager graphicsManager;
        private SpriteBatch spriteBatch;

        public static Texture2D TileSet;
        public static Texture2D OutlineRectangle;

        public static SpriteFont DebugFont;

        private const bool debug = true;

        private Creator mapCreator;

        public Main()
        {
            graphicsManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (debug)
                mapCreator = new Creator();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TileSet = Content.Load<Texture2D>("Tiles/tileSet");
            OutlineRectangle = Content.Load<Texture2D>("Sprites/outlineRectangle");

            DebugFont = Content.Load<SpriteFont>("Fonts/debugFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (debug)
                mapCreator.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (debug)
                mapCreator.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
