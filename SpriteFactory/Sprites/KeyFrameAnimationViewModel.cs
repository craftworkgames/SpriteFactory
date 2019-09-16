using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SpriteFactory.Sprites
{
    public class KeyFrameAnimationViewModel : ViewModel
    {
        public KeyFrameAnimationViewModel()
        {
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetPropertyValue(ref _name, value, nameof(Name));
        }

        private KeyFrameViewModel _selectedKeyFrame;
        public KeyFrameViewModel SelectedKeyFrame
        {
            get => _selectedKeyFrame;
            set
            {
                if(SetPropertyValue(ref _selectedKeyFrame, value, nameof(SelectedKeyFrame)))
                    SelectedKeyFrameIndex = KeyFrames.IndexOf(SelectedKeyFrame);
            }
        }

        public int SelectedKeyFrameIndex { get; private set; }

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