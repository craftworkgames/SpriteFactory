using System.Collections.Generic;

namespace SpriteFactory.Sprites
{
    public class SpritesFile
    {
        public SpritesFile()
        {
            Content = new TilesetContent();
        }

        public string Texture { get; set; }
        public SpriteMode Mode { get; set; } = SpriteMode.Tileset;
        public TilesetContent Content { get; set; }
        public List<SpriteKeyFrameAnimation> Animations { get; set; } = new List<SpriteKeyFrameAnimation>();
    }
}