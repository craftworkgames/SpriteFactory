namespace SpriteFactory.Sprites
{
    public class SpritesFile
    {
        public string Texture { get; set; }
        public SpriteMode Mode { get; set; } = SpriteMode.Tileset;
        public TilesetContent Content { get; set; }
    }
}