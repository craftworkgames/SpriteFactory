namespace SpriteFactory.Sprites
{
    public class KeyFrameAnimationCycle
    {
        public KeyFrameAnimationCycle(int[] frames)
        {
            Frames = frames;
        }

        public int[] Frames { get; }
        public bool IsLooping { get; set; }
        public float FrameDuration { get; set; }
    }
}