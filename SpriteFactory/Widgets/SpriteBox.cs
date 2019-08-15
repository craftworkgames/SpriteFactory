using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace SpriteFactory.Widgets
{
    public class SpriteBox : IMovable
    {
        public SpriteBox(Rectangle rectangle)
        {
            BoundingRectangle = rectangle;
        }

        public Rectangle BoundingRectangle;
        public Vector2 Origin { get; set; }
        public bool IsSelected { get; set; }

        public Vector2 Position
        {
            get => BoundingRectangle.Location.ToVector2();
            set
            {
                BoundingRectangle.X = (int) value.X;
                BoundingRectangle.Y = (int) value.Y;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(BoundingRectangle, IsSelected ? Color.CornflowerBlue : Color.White);

            if (IsSelected)
            {
                var top = BoundingRectangle.Top;
                var left = BoundingRectangle.Left;
                var bottom = BoundingRectangle.Bottom;
                var right = BoundingRectangle.Right;
                spriteBatch.DrawRectangle(new RectangleF(left - 1, top - 1, 2, 2), Color.CornflowerBlue);
                spriteBatch.DrawRectangle(new RectangleF(right - 1, top - 1, 2, 2), Color.CornflowerBlue);
                spriteBatch.DrawRectangle(new RectangleF(left - 1, bottom - 1, 2, 2), Color.CornflowerBlue);
                spriteBatch.DrawRectangle(new RectangleF(right - 1, bottom - 1, 2, 2), Color.CornflowerBlue);
            }
        }
    }
}