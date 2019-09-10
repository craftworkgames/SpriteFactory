using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Xna.Framework;

namespace SpriteFactory.Sprites
{
    public class KeyFrameAnimationViewModel
    {
        public string Name { get; set; }

        public ObservableCollection<KeyFrameViewModel> KeyFrames { get; set; } = new ObservableCollection<KeyFrameViewModel>();

        public override string ToString() => Name;

        public KeyFrameAnimation ToAnimation()
        {
            var keyFrames = KeyFrames.Select(i => i.Index).ToArray();
            return new KeyFrameAnimation(Name, keyFrames);
        }

        public static KeyFrameAnimationViewModel FromAnimation(KeyFrameAnimation animation, Func<string> getImagePath, Func<int, Rectangle> getRectangle)
        {
            var keyFrameViewModels = animation.KeyFrames
                .Select(k => new KeyFrameViewModel(k, getImagePath, getRectangle));

            return new KeyFrameAnimationViewModel
            {
                Name = animation.Name,
                KeyFrames = new ObservableCollection<KeyFrameViewModel>(keyFrameViewModels)
            };
        }
    }
}