using System.Collections.ObjectModel;

namespace SpriteFactory.Sprites
{
    public class SpriteKeyFrameAnimation
    {
        public string Name { get; set; }

        public ObservableCollection<int> KeyFrames { get; set; } = new ObservableCollection<int>();

        public override string ToString() => Name;
    }
}