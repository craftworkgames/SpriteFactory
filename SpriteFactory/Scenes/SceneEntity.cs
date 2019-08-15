using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace SpriteFactory.Scenes
{
    public class SceneEntity : IMovable
    {
        public SceneEntity(Texture2D texture)
        {
            Texture = texture;
            Rotation = 0;
            Origin = texture.Bounds.Center.ToVector2();
            Scale = Vector2.One;
            Effects = SpriteEffects.None;
        }

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects Effects { get; set; }
        public Rectangle BoundingRectangle => new Rectangle((int) (Position.X - Origin.X), (int) (Position.Y - Origin.Y), Texture.Width, Texture.Height);
    }
}