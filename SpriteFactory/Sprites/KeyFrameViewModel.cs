using System.Windows;
using Microsoft.Xna.Framework;

namespace SpriteFactory.Sprites
{
    public class KeyFrameViewModel
    {
        public KeyFrameViewModel(int index, string imagePath, Rectangle rectangle)
        {
            Index = index;
            ImagePath = imagePath ?? string.Empty;
            Region = new Int32Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public int Index { get; }
        public string ImagePath { get; }
        public Int32Rect Region { get; }
    }
}