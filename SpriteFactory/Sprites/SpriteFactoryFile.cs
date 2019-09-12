using System.Collections.Generic;

namespace SpriteFactory.Sprites
{
    public class SpriteFactoryFile
    {
        public SpriteFactoryFile()
        {
        }

        public string Texture { get; set; }
        public int TileWidth { get; set; } = 32;
        public int TileHeight { get; set; } = 32;
        public List<KeyFrameAnimation> Animations { get; set; } = new List<KeyFrameAnimation>();
    }
}