using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpriteFactory.MonoGameControls
{
    public class MouseStateArgs
    {
        private readonly IInputElement _element;
        private readonly MouseEventArgs _args;

        public MouseStateArgs(IInputElement element, MouseEventArgs args)
        {
            _element = element;
            _args = args;
        }

        public Vector2 Position => _args.GetPosition(_element).ToVector2();
        public ButtonState LeftButton => ConvertButtonState(_args.LeftButton);
        public ButtonState RightButton => ConvertButtonState(_args.RightButton);
        public ButtonState MiddleButton => ConvertButtonState(_args.MiddleButton);

        private static ButtonState ConvertButtonState(MouseButtonState state) => state == MouseButtonState.Pressed ? ButtonState.Pressed : ButtonState.Released;
    }
}