using System;
using System.Windows;
using Microsoft.Xna.Framework;

namespace SpriteFactory.Sprites
{
    public class KeyFrameViewModel
    {
        private readonly Func<string> _getImagePath;
        private readonly Func<int, Rectangle> _getRectangle;

        public KeyFrameViewModel(int index, Func<string> getImagePath, Func<int,Rectangle> getRectangle)
        {
            Index = index;
            _getImagePath = getImagePath;
            _getRectangle = getRectangle;
        }

        public int Index { get; }
        public string ImagePath => _getImagePath();

        public Int32Rect Region
        {
            get
            {
                var rectangle = _getRectangle(Index);
                return new Int32Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }
        
    }
}