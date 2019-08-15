using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteFactory.Widgets
{
    public class SpriteSheetBox
    {
        public SpriteSheetBox(Texture2D texture)
        {
            Texture = texture;
            Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            BoundingRectangle = new Rectangle((int) (-Texture.Width / 2f), (int) (-Texture.Height / 2f), Texture.Width, Texture.Height);
            SpriteBoxes = new List<SpriteBox>();
        }

        public Texture2D Texture { get; }
        public Vector2 Origin { get; }
        public Rectangle BoundingRectangle { get; }
        public List<SpriteBox> SpriteBoxes { get; }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Vector2.Zero, null, Color.White, 0, Origin, Vector2.One, SpriteEffects.None, 0);

            foreach (var spriteBox in SpriteBoxes)
                spriteBox.Draw(spriteBatch);
        }

        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}