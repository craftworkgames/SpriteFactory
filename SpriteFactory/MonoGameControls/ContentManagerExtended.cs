using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteFactory.MonoGameControls
{
    public class ContentManagerExtended : ContentManager
    {
        private readonly Dictionary<string, object> _rawContentCache = new Dictionary<string, object>();
        private readonly Dictionary<Type, Func<string, object>> _loaders;

        public ContentManagerExtended(IServiceProvider serviceProvider, string rootDirectory = "Content")
            : base(serviceProvider, rootDirectory)
        {
            _loaders = new Dictionary<Type, Func<string, object>>
            {
                {typeof(Texture2D), LoadTexture}
            };
        }

        public IGraphicsDeviceService GraphicsDeviceService => (IGraphicsDeviceService) ServiceProvider.GetService(typeof(IGraphicsDeviceService));
        public GraphicsDevice GraphicsDevice => GraphicsDeviceService?.GraphicsDevice;
        
        protected override void Dispose(bool disposing)
        {
            foreach (var texture in _rawContentCache.Values.OfType<IDisposable>())
                texture.Dispose();

            base.Dispose(disposing);
        }

        public T LoadRaw<T>(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            if (_rawContentCache.TryGetValue(fullPath, out var content))
                return (T)content;

            if (_loaders.TryGetValue(typeof(T), out var loader))
            {
                var newContent = loader(filePath);
                _rawContentCache[fullPath] = newContent;
                return (T)newContent;
            }

            throw new NotSupportedException($"{typeof(T)} is not supported in {nameof(LoadRaw)}");
        }

        private Texture2D LoadTexture(string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                var newTexture = Texture2D.FromStream(GraphicsDeviceService.GraphicsDevice, fileStream);
                PremultiplyTexture(newTexture);
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