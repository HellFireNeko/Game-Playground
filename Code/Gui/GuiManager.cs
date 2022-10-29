using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.Input.InputListeners;

using Serilog;

namespace GamePlayground.Code.Gui
{
    public class GuiManager : DrawableGameComponent
    {
        private new Game1 Game => (Game1)base.Game;

        private readonly List<GuiComponent> Components;

        public GuiComponent FocusedComponent = null;

        public GuiManager(Game game) : base(game) 
        {
            Components = new List<GuiComponent>();
        }

        public GuiManager(Game game, params GuiComponent[] components) : base(game)
        {
            Components = new List<GuiComponent>(components);
        }

        private Vector2 _mousePosition;
        public Vector2 MousePosition => _mousePosition;

        public override void Initialize()
        {
            base.Initialize();

            foreach (var component in Components)
            {
                component.SetGuiManager(this);
            }

            Log.Information("Gui Manager Initialized");
        }

        public void SetFocusedObject(GuiComponent component)
        {
            FocusedComponent = component;

            if (component != null)
                CurrentIndex = Components.IndexOf(component);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            foreach (var component in Components)
            {
                component.Initialize();
            }

            Game.MouseListener.MouseMoved += OnMouseMoved;
            Game.MouseListener.MouseDown += OnMouseDown;
            Game.MouseListener.MouseUp += OnMouseUp;

            Game.KeyboardListener.KeyPressed += OnKeyboardPressed;
            Game.KeyboardListener.KeyReleased += OnKeyboardReleased;

            Game.GamePadListener.ButtonDown += OnGamepadButtonDown;
            Game.GamePadListener.ButtonUp += OnGamepadButtonUp;

            Log.Information("Gui Manager Loaded");
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (var component in Components)
            {
                component.Dispose();
            }

            Game.MouseListener.MouseMoved -= OnMouseMoved;
            Game.MouseListener.MouseDown -= OnMouseDown;
            Game.MouseListener.MouseUp -= OnMouseUp;

            Game.KeyboardListener.KeyPressed -= OnKeyboardPressed;
            Game.KeyboardListener.KeyReleased -= OnKeyboardReleased;

            Game.GamePadListener.ButtonDown -= OnGamepadButtonDown;
            Game.GamePadListener.ButtonUp -= OnGamepadButtonUp;

            Log.Information("Gui Manager Unloaded");
        }

        private void OnGamepadButtonDown(object sender, GamePadEventArgs args)
        {
            if (args.Button == Buttons.DPadUp)
            {
                if (CurrentIndex - 1 < 0)
                    CurrentIndex = Components.Count;
                else
                    CurrentIndex--;

                FocusedComponent = Components[CurrentIndex];

                TabReleased = false;
            }
            if (args.Button == Buttons.DPadDown)
            {
                if (CurrentIndex + 1 >= Components.Count)
                    CurrentIndex = 0;
                else
                    CurrentIndex++;

                FocusedComponent = Components[CurrentIndex];

                TabReleased = false;
            }

            if (args.Button == Buttons.A)
            {
                FocusedComponent.SetActive(true);
            }
        }

        private void OnGamepadButtonUp(object sender, GamePadEventArgs args)
        {
            if (args.Button == Buttons.DPadUp)
            {
                TabReleased = true;
            }
            if (args.Button == Buttons.DPadDown)
            {
                TabReleased = true;
            }

            if (args.Button == Buttons.A)
            {
                Components.ForEach(x => x.SetActive(false));
            }
        }

        private void OnMouseMoved(object sender, MouseEventArgs args)
        {
            _mousePosition = Game.CurrentCamera.ScreenToWorld(args.Position.ToVector2());
        }

        private void OnMouseDown(object sender, MouseEventArgs args)
        {
            if (args.Button == MonoGame.Extended.Input.MouseButton.Left)
            {
                FocusedComponent = null;
                Components.ForEach(x =>
                {
                    if (x.Position.Contains(Game.CurrentCamera.ScreenToWorld(args.Position.ToVector2())))
                    {
                        SetFocusedObject(x);
                        x.MouseDown();
                    }
                });
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs args)
        {
            if (args.Button == MonoGame.Extended.Input.MouseButton.Left)
                Components.ForEach(x =>
                {
                    if (x.Position.Contains(Game.CurrentCamera.ScreenToWorld(args.Position.ToVector2())))
                    {
                        x.MouseUp();
                    }
                });
        }

        private bool TabReleased = true;

        private int CurrentIndex = -1;

        private void OnKeyboardPressed(object sender, KeyboardEventArgs args)
        {
            if (args.Key == Keys.Tab && TabReleased)
            {
                if (CurrentIndex + 1 >= Components.Count)
                    CurrentIndex = 0;
                else
                    CurrentIndex++;

                FocusedComponent = Components[CurrentIndex];

                TabReleased = false;
            }
            else if (args.Key == Keys.Enter)
                FocusedComponent.SetActive(true);
        }

        private void OnKeyboardReleased(object sender, KeyboardEventArgs args)
        {
            if (args.Key == Keys.Tab)
                TabReleased = true;
            else if (args.Key == Keys.Enter)
                Components.ForEach(x => x.SetActive(false));
        }

        public void AddGuiComponent(GuiComponent component)
        {
            if (string.IsNullOrEmpty(component.Name))
                return;

            component.SetGuiManager(this);
            Components.Add(component);
        }

        public T CreateGuiComponent<T>(string name, int x, int y, int width, int height) where T : GuiComponent, new()
        {
            return CreateGuiComponent<T>(name, new Rectangle(x, y, width, height));
        }

        public T CreateGuiComponent<T>(string name, Rectangle position) where T : GuiComponent, new()
        {
            var guiComponent = new T
            {
                Position = position,
                Name = name
            };
            AddGuiComponent(guiComponent);
            Log.Information("New Gui Component created, name: {Name}, position: {Position}", name, position);
            return guiComponent;
        }

        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                foreach (var component in Components)
                {
                    component.Update(gameTime);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                Game.SpriteBatch.Begin(transformMatrix: Game.CurrentCamera.GetViewMatrix());

                foreach (var component in Components)
                {
                    component.Draw(gameTime);
                }

                Game.SpriteBatch.End();
            }
        }
    }
}
