using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Input.InputListeners;

using GamePlayground.Code.Screens;

namespace GamePlayground
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch => _spriteBatch;
        public Camera<Vector2> CurrentCamera;

        private readonly ScreenManager _screenManager;

        private SpriteFont _font;
        public SpriteFont Font => _font;

        private readonly GamePadListener _gamePadListener;
        public GamePadListener GamePadListener => _gamePadListener;
        private readonly KeyboardListener _keyboardListener;
        public KeyboardListener KeyboardListener => _keyboardListener;
        private readonly MouseListener _mouseListener;
        public MouseListener MouseListener => _mouseListener;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            _screenManager = new ScreenManager();
            Components.Add(_screenManager);

            _gamePadListener = new GamePadListener();
            _keyboardListener = new KeyboardListener();
            _mouseListener = new MouseListener();

            Components.Add(new InputListenerComponent(this, _gamePadListener, _keyboardListener, _mouseListener));
        }

        protected override void Initialize()
        {
            base.Initialize();
            LoadScreen(new MainMenu(this));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>("Font");
        }

        public void LoadScreen(GameScreen screen)
        {
            _screenManager.LoadScreen(screen, new FadeTransition(GraphicsDevice, Color.Black));
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}