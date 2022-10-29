using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Serilog;

using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Collisions;

using GamePlayground.Code.Gui;
using GamePlayground.Code.Entities;

namespace GamePlayground.Code.Screens
{
    public class GamePlayScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;

        public GamePlayScreen(Game1 game) : base(game) { }

        private TiledMap _tilemap;
        private TiledMapRenderer _tiledMapRenderer;

        private Vector2 _cameraPosition = Vector2.Zero;

        private CollisionWorld _collisionWorld;

        private Player player;

        private OrthographicCamera _gameCamera;

        public override void Initialize()
        {
            base.Initialize();

            var viewportAdapter = new ScalingViewportAdapter(GraphicsDevice, 800, 480);
            Game.CurrentCamera = new OrthographicCamera(viewportAdapter);

            _gameCamera = new OrthographicCamera(viewportAdapter)
            {
                Zoom = 2
            };
        }

        public override void LoadContent()
        {
            _tilemap = Game.Content.Load<TiledMap>("Map");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tilemap);

            var layer = _tilemap.TileLayers[0];

            var data = layer.Tiles.Select(t => (int)t.GlobalTileIdentifierWithFlags).ToArray();

            _collisionWorld = new CollisionWorld(new Vector2(0, 9.81f));
            _collisionWorld.CreateGrid(data, layer.Width, layer.Height, layer.TileWidth, layer.TileHeight);

            var startPosition = _tilemap.ObjectLayers[0].Objects.First(x => x.Name == "Spawn Point").Position;

            player = new Player(Game) { Position = startPosition, Size = new Vector2(16, 16) };

            AddNewEntity(player);

            Game.KeyboardListener.KeyPressed += OnKeyPressed;
            Game.KeyboardListener.KeyReleased += OnKeyReleased;

            Game.GamePadListener.ButtonDown += OnGamepadButtonDown;
            Game.GamePadListener.ButtonUp += OnGamepadButtonUp;

            Log.Information("Scene Loaded {Name}", "Game Play Screen");

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            Unload = true;

            _tiledMapRenderer.Dispose();

            Game.KeyboardListener.KeyPressed -= OnKeyPressed;
            Game.KeyboardListener.KeyReleased -= OnKeyReleased;

            Game.GamePadListener.ButtonDown -= OnGamepadButtonDown;
            Game.GamePadListener.ButtonUp -= OnGamepadButtonUp;

            Log.Information("Scene Unloaded {Name}", "Game Play Screen");

            base.UnloadContent();
        }

        private void OnGamepadButtonDown(object sender, GamePadEventArgs args)
        {
            switch (args.Button)
            {
                case Buttons.DPadLeft:
                    state |= InputState.Left;
                    state &= ~InputState.Right;
                    break;

                case Buttons.DPadRight:
                    state |= InputState.Right;
                    state &= ~InputState.Left;
                    break;

                case Buttons.A:
                    state |= InputState.Jump;
                    break;

                case Buttons.Start:
                    ReturnButtonClicked();
                    break;
            }
        }

        private void OnGamepadButtonUp(object sender, GamePadEventArgs args)
        {
            switch (args.Button)
            {
                case Buttons.DPadLeft:
                    state &= ~InputState.Left;
                    break;

                case Buttons.DPadRight:
                    state &= ~InputState.Right;
                    break;

                case Buttons.A:
                    state &= ~InputState.Jump;
                    break;

                case Buttons.Start:
                    ReturnButtonClicked();
                    break;
            }
        }

        private InputState state = InputState.None;

        private void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.D:
                    state |= InputState.Left;
                    state &= ~InputState.Right;
                    break;

                case Keys.A:
                    state |= InputState.Right;
                    state &= ~InputState.Left;
                    break;

                case Keys.Space:
                    state |= InputState.Jump;
                    break;

                case Keys.Escape:
                    ReturnButtonClicked();
                    break;
            }
        }

        private void OnKeyReleased(object sender, KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.D:
                    state &= ~InputState.Left;
                    break;

                case Keys.A:
                    state &= ~InputState.Right;
                    break;

                case Keys.Space:
                    state &= ~InputState.Jump;
                    break;
            }
        }

        private void AddNewEntity(IEntity entity)
        {
            _collisionWorld.CreateActor(entity);
        }

        private bool Unload = false;

        private void ReturnButtonClicked()
        {
            Game.LoadScreen(new MainMenu(Game));
        }

        private void MoveCamera()
        {
            _cameraPosition = player.Position;
        }

        private RectangleF MapBounds
        {
            get
            {
                var x = 0;
                var y = 0;
                var width = _tilemap.Width * _tilemap.TileWidth;
                var height = _tilemap.Height * _tilemap.TileHeight;
                return new Rectangle(x, y, width, height);
            }
        }

        private Point2 GetCameraPositionInBounds(RectangleF cameraBounds)
        {
            var newBounds = cameraBounds;

            if (cameraBounds.X < MapBounds.X) newBounds.X = MapBounds.X;
            if (cameraBounds.Y < MapBounds.Y) newBounds.Y = MapBounds.Y;
            if (cameraBounds.Right > MapBounds.Right) newBounds.X = MapBounds.Right - cameraBounds.Width;
            if (cameraBounds.Bottom > MapBounds.Bottom) newBounds.Y = MapBounds.Bottom - cameraBounds.Height;

            return newBounds.Center;
        }

        private Vector2 previousVelocity = Vector2.Zero;

        private Vector2 GetNewVelocity(IEntity ent)
        {
            var velocity = ent.Velocity;

            if (state.HasFlag(InputState.Jump) && ent.Velocity.Y == 0 && previousVelocity.Y == 0)
            {
                // Jump
                velocity.Y -= 3;
            }

            velocity.X = 0;

            if (state.HasFlag(InputState.Left))
            {
                velocity.X = 2;
            }
            else if (state.HasFlag(InputState.Right))
            {
                velocity.X = -2;
            }

            return velocity;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Unload)
            {
                MoveCamera();

                _tiledMapRenderer?.Update(gameTime);
                
                _gameCamera.LookAt(_cameraPosition);
                _cameraPosition = GetCameraPositionInBounds(_gameCamera.BoundingRectangle);
                _gameCamera.LookAt(_cameraPosition);

                player.Update(gameTime);

                if (MapBounds.Contains(player.Position) is false)
                    ReturnButtonClicked();

                player.Velocity = GetNewVelocity(player);

                _collisionWorld.Update(gameTime);

                previousVelocity = player.Velocity;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _tiledMapRenderer?.Draw(_gameCamera.GetViewMatrix());
            Game.SpriteBatch.Begin(transformMatrix: _gameCamera.GetViewMatrix());

            player.Draw(Game.SpriteBatch);

            Game.SpriteBatch.End();
        }

        [Flags]
        private enum InputState
        {
            None = 0,
            Jump = 1,
            Left = 2,
            Right = 4,
        }
    }
}
