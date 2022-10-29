using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Serilog;

using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;

using GamePlayground.Code.Gui;

namespace GamePlayground.Code.Screens
{
    public class MainMenu : GameScreen
    {
        private new Game1 Game => (Game1) base.Game;

        public MainMenu(Game1 game) : base(game) { }

        private GuiManager _guiManager;

        private GuiButtonColors _buttonColors;

        public override void Initialize()
        {
            base.Initialize();

            var viewportAdapter = new ScalingViewportAdapter(GraphicsDevice, 800, 480);
            Game.CurrentCamera = new OrthographicCamera(viewportAdapter);

            _buttonColors = new GuiButtonColors()
            {
                DefaultColor = Color.White,
                HoverColor = Color.Gray,
                PressedColor = Color.DarkGray,
                FocusColor = Color.Gray
            };
        }

        public override void LoadContent()
        {
            var playButton = new GuiButton(Game, "Play Button", _buttonColors, Color.Black);
            playButton.Clicked += PlayButton_Clicked;
            playButton.Text = "Play";
            playButton.Position = new RectangleF(300, 200, 200, 50);
            playButton.BorderThickness = 1f;

            var exitButton = new GuiButton(Game, "Exit Button", _buttonColors, Color.Black);
            exitButton.Clicked += ExitButton_Clicked;
            exitButton.Text = "Exit";
            exitButton.Position = new RectangleF(300, 260, 200, 50);
            exitButton.BorderThickness = 1f;

            _guiManager = new GuiManager(Game, playButton, exitButton);
            _guiManager.Initialize();

            Log.Information("Scene Loaded {Name}", "Main Menu");

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _guiManager.Dispose();

            Log.Information("Scene Unloaded {Name}", "Main Menu");

            base.UnloadContent();
        }

        private void ExitButton_Clicked()
        {
            Game.Exit();
        }

        private void PlayButton_Clicked()
        {
            Game.LoadScreen(new GamePlayScreen(Game));
        }

        public override void Update(GameTime gameTime)
        {
            _guiManager?.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _guiManager.Draw(gameTime);
        }
    }
}
