using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace SpriteFactory.Widgets
{
    public enum ResizeHandle
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Left,
        Right,
        Top,
        Bottom,
        Centre,
        None
    }

    public class ResizableBox : IMovable, IRectangular
    {
        public ResizableBox(Rectangle rectangle)
        {
            BoundingRectangle = rectangle;
        }

        public Vector2 Position { get; set; }
        public Rectangle BoundingRectangle { get; private set; }
        public ResizeHandle DragHandle { get; private set; }

        private Rectangle _mouseDownRectangle;
        private Vector2 _mouseDownPosition;

        public bool IsGrabbed => DragHandle != ResizeHandle.None;

        public bool TryGrab(Vector2 worldPosition)
        {
            _mouseDownRectangle = BoundingRectangle;
            _mouseDownPosition = worldPosition;
            DragHandle = GetResizeHandle(worldPosition);
            return IsGrabbed;
        }

        public void Release()
        {
            DragHandle = ResizeHandle.None;
        }

        public void Resize(Vector2 worldPosition)
        {
            if (DragHandle == ResizeHandle.Centre)
            {
                var offset = worldPosition - _mouseDownPosition;
                var x = (int)(_mouseDownRectangle.X + offset.X);
                var y = (int)(_mouseDownRectangle.Y + offset.Y);

                BoundingRectangle = new Rectangle(x, y, _mouseDownRectangle.Width, _mouseDownRectangle.Height);
            }
            else
            {
                var right = DragHandle == ResizeHandle.Right ||
                            DragHandle == ResizeHandle.TopRight ||
                            DragHandle == ResizeHandle.BottomRight ?
                    worldPosition.X : _mouseDownRectangle.Right;

                var left = DragHandle == ResizeHandle.Left ||
                           DragHandle == ResizeHandle.TopLeft ||
                           DragHandle == ResizeHandle.BottomLeft ?
                    worldPosition.X : _mouseDownRectangle.Left;

                var top = DragHandle == ResizeHandle.Top ||
                          DragHandle == ResizeHandle.TopLeft ||
                          DragHandle == ResizeHandle.TopRight ?
                    worldPosition.Y : _mouseDownRectangle.Top;

                var bottom = DragHandle == ResizeHandle.Bottom ||
                             DragHandle == ResizeHandle.BottomLeft ||
                             DragHandle == ResizeHandle.BottomRight
                    ? worldPosition.Y : _mouseDownRectangle.Bottom;

                var width = Math.Abs(left - right);
                var height = Math.Abs(top - bottom);

                BoundingRectangle = new Rectangle((int)left, (int)top, (int)width, (int)height);
            }
        }

        public ResizeHandle GetResizeHandle(Vector2 position)
        {
            const float distance = 4f;
            var x = (int)position.X;
            var y = (int)position.Y;
            var rectangle = BoundingRectangle;
            var left = x >= rectangle.Left - distance && x <= rectangle.Left + distance;
            var right = x >= rectangle.Right - distance && x <= rectangle.Right + distance;
            var top = y >= rectangle.Top - distance && y <= rectangle.Top + distance;
            var bottom = y >= rectangle.Bottom - distance && y <= rectangle.Bottom + distance;

            if (top && left)
                return ResizeHandle.TopLeft;

            if (bottom && right)
                return ResizeHandle.BottomRight;

            if (top && right)
                return ResizeHandle.TopRight;

            if (bottom && left)
                return ResizeHandle.BottomLeft;
            
            if (left)
                return ResizeHandle.Left;

            if (right)
                return ResizeHandle.Right;

            if (top)
                return ResizeHandle.Top;

            if (bottom)
                return ResizeHandle.Bottom;

            if (rectangle.Contains(position))
                return ResizeHandle.Centre;

            return ResizeHandle.None;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var top = BoundingRectangle.Top;
            var left = BoundingRectangle.Left;
            var bottom = BoundingRectangle.Bottom;
            var right = BoundingRectangle.Right;

            spriteBatch.DrawRectangle(BoundingRectangle, Color.CornflowerBlue);
            spriteBatch.FillRectangle(left - 1, top - 1, 4, 4, Color.CornflowerBlue);
            spriteBatch.FillRectangle(right - 3, top - 1, 4, 4, Color.CornflowerBlue);
            spriteBatch.FillRectangle(left - 1, bottom - 3, 4, 4, Color.CornflowerBlue);
            spriteBatch.FillRectangle(right - 3, bottom - 3, 4, 4, Color.CornflowerBlue);
        }
    }
}