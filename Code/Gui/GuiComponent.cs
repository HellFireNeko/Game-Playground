using System;

using Microsoft.Xna.Framework;

using MonoGame.Extended;

namespace GamePlayground.Code.Gui
{
#pragma warning disable CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
    public abstract class GuiComponent : IDisposable, IEquatable<GuiComponent>
#pragma warning restore CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
    {
        public Game1 Game { get; }
        public GuiManager GuiManager { get; private set; }

        private bool _isInitialized;

        public string Name { get; set; }

        protected GuiComponent(Game game, string name, GuiManager guiManager)
        {
            Game = game as Game1;
            Name = name;
            GuiManager = guiManager;
        }

        public void SetGuiManager(GuiManager guiManager)
        {
            GuiManager = guiManager;
        }

        public bool IsPositionCameraRelative { get; set; }

        public RectangleF Position { get; set; }

        public RectangleF RelativePosition
        {
            get
            {
                return new RectangleF(Position.Position + Game.CurrentCamera.Position, Position.Size);
            }
        }

        public RectangleF GetPosition()
        {
            return IsPositionCameraRelative switch
            {
                true => RelativePosition,
                false => Position,
            };
        }

        public abstract void SetActive(bool active);

        public virtual void MouseDown() 
        {
            SetActive(true);
        }

        public virtual void MouseUp()
        {
            SetActive(false);
        }

        public virtual void Dispose()
        {
            if (_isInitialized)
            {
                UnloadContent();
                _isInitialized = false;

                GC.SuppressFinalize(this);
            }
        }

        public virtual void Initialize()
        {
            if (!_isInitialized)
            {
                LoadContent();
                _isInitialized = true;
            }
        }

        protected virtual void LoadContent() { }
        protected virtual void UnloadContent() { }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);

        public bool Equals(GuiComponent other)
        {
            if (other == null || string.IsNullOrEmpty(other.Name) || string.IsNullOrEmpty(Name))
                return false;
            return Name == other.Name;
        }
    }
}
