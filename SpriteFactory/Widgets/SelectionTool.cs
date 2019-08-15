using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace SpriteFactory.Widgets
{
    public class SelectionTool
    {
        public SelectionTool(Vector2 position)
        {
            Start = position;
            BoundingRectangle = new RectangleF(position, Size2.Empty);
        }

        public Vector2 Start { get; private set; }
        public RectangleF BoundingRectangle { get; private set; }

        public void OnMouseMove(Vector2 position)
        {
            var width = Math.Abs(Start.X - position.X);
            var height = Math.Abs(Start.Y - position.Y);
            var minX = MathHelper.Min(Start.X, position.X);
            var minY = MathHelper.Min(Start.Y, position.Y);

            BoundingRectangle = new RectangleF(minX, minY, width, height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(BoundingRectangle, Color.LightGreen);
        }
    }
}