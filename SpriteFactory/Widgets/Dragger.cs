using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SpriteFactory.Widgets
{
    public class Dragger
    {
        private readonly IMovable _target;
        private readonly Vector2 _initialMousePosition;
        private readonly Vector2 _initialTargetPosition;

        public Dragger(IMovable target, Vector2 initialMousePosition)
        {
            _target = target;
            _initialMousePosition = initialMousePosition;
            _initialTargetPosition = _target.Position;
        }

        public void OnMouseMove(Vector2 mousePosition)
        {
            var vector = mousePosition - _initialMousePosition;
            _target.Position = _initialTargetPosition + vector;
        }
    }
}