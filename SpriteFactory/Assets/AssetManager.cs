using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteFactory.Assets
{
    public class AssetManager : IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

        public AssetManager(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void Dispose()
        {
            foreach (var texture in _textures.Values)
                texture.Dispose();
        }

        public Texture2D LoadTexture(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (_textures.TryGetValue(fullPath, out var texture))
                return texture;

            using (var fileStream = File.OpenRead(filePath))
            {
                var newTexture = Texture2D.FromStream(_graphicsDevice, fileStream);
                PremultiplyTexture(newTexture);
                _textures[fullPath] = newTexture;
                return newTexture;
            }
        }

        private static void PremultiplyTexture(Texture2D texture)
        {
            var data = new Color[texture.Width * texture.Height];

            texture.GetData(data);

            for (var i = 0; i < data.Length; i++)
                data[i] = Color.FromNonPremultiplied(data[i].R, data[i].G, data[i].B, data[i].A);

            texture.SetData(data);
        }
    }
}