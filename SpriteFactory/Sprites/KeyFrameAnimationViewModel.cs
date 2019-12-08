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
        
        private bool _isLooping;
        private float _frameDuration = 0.2f;

        public bool IsLooping
        {
            get => _isLooping;
            set => SetPropertyValue(ref _isLooping, value, nameof(IsLooping));
        }

        public float FrameDuration
        {
            get => _frameDuration;
            set => SetPropertyValue(ref _frameDuration, value, nameof(FrameDuration));
        }

        public override string ToString() => Name;

        public KeyFrameAnimationCycle ToAnimationCycle()
        {
            var keyFrames = KeyFrames
                .Select(i => i.Index)
                .ToArray();

            return new KeyFrameAnimationCycle(keyFrames)
            {
                IsLooping = IsLooping,
                FrameDuration = FrameDuration
            };
        }

        public static KeyFrameAnimationViewModel FromAnimation(string name, KeyFrameAnimationCycle cycle, Func<string> getImagePath, Func<int, Rectangle> getRectangle)
        {
            var keyFrameViewModels = cycle.Frames
                .Select(k => new KeyFrameViewModel(k, getImagePath, getRectangle));

            return new KeyFrameAnimationViewModel
            {
                Name = name,
                KeyFrames = new ObservableCollection<KeyFrameViewModel>(keyFrameViewModels),
                IsLooping = cycle.IsLooping,
                FrameDuration = cycle.FrameDuration
            };
        }
    }
}