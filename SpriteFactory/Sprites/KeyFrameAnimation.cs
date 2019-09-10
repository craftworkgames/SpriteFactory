namespace SpriteFactory.Sprites
{
    public class KeyFrameAnimation
    {
        public KeyFrameAnimation(string name, int[] keyFrames)
        {
            Name = name;
            KeyFrames = keyFrames;
        }

        public string Name { get; }

        public int[] KeyFrames { get; }

        public override string ToString() => Name;
    }
}