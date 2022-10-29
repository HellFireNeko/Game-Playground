using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Serilog;

using MonoGame.Extended;

namespace GamePlayground.Code.Gui
{
    public class GuiButton : GuiComponent
    {
        public event OnButtonClickedDelegate Clicked;

        public delegate void OnButtonClickedDelegate();

        private new Game1 Game => (Game1)base.Game;

        public GuiButton(Game1 game, string name, GuiButtonColors buttonColors, Color textColor) : this(game, name, null, buttonColors, textColor) { }

        public GuiButton(Game1 game, string name, GuiManager guiManager, GuiButtonColors buttonColors, Color textColor) : base(game, name, guiManager)
        {
            ButtonColors = buttonColors;
            TextColor = textColor;
        }

        public string Text;
        public Color TextColor;
        public float BorderThickness = 0;
        public Color BorderColor = Color.Black;
        private GuiButtonColors ButtonColors;
        public GuiButtonStates ButtonState = GuiButtonStates.Default;

        private GuiButtonStates SetFlag(GuiButtonStates a)
        {
            return ButtonState | a;
        }

        private GuiButtonStates UnsetFlag(GuiButtonStates a)
        {
            return ButtonState & (~a);
        }

        private bool _pressed;
        private bool _mousePressed;
        public bool Pressed => _pressed || _mousePressed;

        public override void SetActive(bool active)
        {
            _pressed = active;
        }

        public override void MouseDown()
        {
            _mousePressed = true;
        }

        public override void MouseUp()
        {
            _mousePressed = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (GuiManager != null)
            {
                if (Position.Contains(GuiManager.MousePosition))
                    ButtonState = SetFlag(GuiButtonStates.Hover);
                else
                    ButtonState = UnsetFlag(GuiButtonStates.Hover);

                if (GuiManager.FocusedComponent != null)
                    if (GuiManager.FocusedComponent.Equals(this))
                        ButtonState = SetFlag(GuiButtonStates.Focus);
                    else
                        ButtonState = UnsetFlag(GuiButtonStates.Focus);
                else
                    ButtonState = UnsetFlag(GuiButtonStates.Focus);

                if (Pressed)
                    ButtonState = SetFlag(GuiButtonStates.Pressed);
                else if (ButtonState.HasFlag(GuiButtonStates.Pressed))
                {
                    ButtonState = UnsetFlag(GuiButtonStates.Pressed);
                    Log.Information("Button Clicked: {Name}", Name);
                    Clicked?.Invoke();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.FillRectangle(GetPosition(), PickColor());
            Game.SpriteBatch.DrawRectangle(GetPosition(), BorderColor, BorderThickness);
            if (Text.Length > 0)
            {
                Game.SpriteBatch.DrawString(Game.Font, Text, GetTextPosition(Game.Font), TextColor);
            }
        }

        private Vector2 GetTextPosition(SpriteFont targetFont)
        {
            return GetPosition().Center - (targetFont.MeasureString(Text) / 2f);
        }

        private Color PickColor()
        {
            if (ButtonState.HasFlag(GuiButtonStates.Pressed))
                return ButtonColors.PressedColor;
            if (ButtonState.HasFlag(GuiButtonStates.Hover))
                return ButtonColors.HoverColor;
            if (ButtonState.HasFlag(GuiButtonStates.Focus))
                return ButtonColors.FocusColor;            
            return ButtonColors.DefaultColor;
        }
    }

    [Flags]
    public enum GuiButtonStates
    {
        Default = 0,
        Hover = 1,
        Pressed = 2,
        Focus = 4,
    }

    public struct GuiButtonColors
    {
        public Color DefaultColor { get; set; }
        public Color HoverColor { get; set; }
        public Color PressedColor { get; set; }
        public Color FocusColor { get; set; }
    }
}
