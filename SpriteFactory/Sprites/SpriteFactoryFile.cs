using System.Collections.Generic;

namespace SpriteFactory.Sprites
{
    public class TextureAtlas
    {
        public string Texture { get; set; }
        public int RegionWidth { get; set; } = 32;
        public int RegionHeight { get; set; } = 32;
    }

    public class SpriteFactoryFile
    {
        public TextureAtlas TextureAtlas { get; set; } = new TextureAtlas();
        public Dictionary<string, KeyFrameAnimationCycle> Cycles { get; set; } = new Dictionary<string, KeyFrameAnimationCycle>();
    }
}