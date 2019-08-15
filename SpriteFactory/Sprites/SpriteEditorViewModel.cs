using System;
using System.IO;
using System.Windows.Input;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SpriteFactory.Assets;

namespace SpriteFactory.Sprites
{
    public class SpriteEditorViewModel : ViewModel
    {
        private readonly AssetManager _assetManager;
        private readonly SpriteBatch _spriteBatch;

        public SpriteEditorViewModel(AssetManager assetManager, GraphicsDevice graphicsDevice)
        {
            _assetManager = assetManager;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            _camera = new OrthographicCamera(graphicsDevice);
            _camera.LookAt(Vector2.Zero);

            SelectTextureCommand = new Command(SelectTexture);
            AutoDetectCommand = new Command(AutoDetect);
        }

        private OrthographicCamera _camera;

        public Vector2 Origin => Texture != null ? new Vector2(Texture.Width / 2f, Texture.Height / 2f) : Vector2.Zero;
        public Rectangle BoundingRectangle => Texture?.Bounds ?? Rectangle.Empty;

        private string _texturePath;
        public string TexturePath
        {
            get => _texturePath;
            private set
            {
                if (SetPropertyValue(ref _texturePath, value, nameof(TexturePath)))
                    TextureName = Path.GetFileName(_texturePath);
            }
        }
        
        private string _textureName;
        public string TextureName
        {
            get => _textureName ?? "(no texture selected)";
            private set => SetPropertyValue(ref _textureName, value, nameof(TextureName));
        }

        private Texture2D _texture;
        public Texture2D Texture
        {
            get => _texture;
            private set
            {
                if (_texture != value)
                {
                    _texture = value;
                    SetPropertyValue(ref _texture, value, nameof(Texture));
                    OnTextureChanged?.Invoke(value);
                }
            }
        }

        public Action<Texture2D> OnTextureChanged { get; set; }

        public ICommand SelectTextureCommand { get; }

        public ICommand AutoDetectCommand { get; }

        private async void SelectTexture()
        {
            var openFileService = DependencyResolver.Resolve<IOpenFileService>();
            openFileService.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";

            if (await openFileService.DetermineFileAsync())
            {
                TexturePath = openFileService.FileName;
                Texture = _assetManager.LoadTexture(TexturePath);
            }
        }

        private void AutoDetect()
        {
            var data = new Color[Texture.Width * Texture.Height];
            Texture.GetData(data);

            for (var y = 0; y < Texture.Height; y++)
            {
                for (var x = 0; x < Texture.Width; x++)
                {
                    var color = data[y * Texture.Height + x];

                    if (color.A != 0)
                    {
                        if(_autoRectangle.IsEmpty)
                            _autoRectangle = new Rectangle(x, y, 1, 1);
                    }
                }
            }
        }

        private Rectangle _autoRectangle;

        public void Draw()
        {
            if(Texture == null)
                return;
            
            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(Texture, Vector2.Zero, null, Color.White, 0, Origin, Vector2.One, SpriteEffects.None, 0);
            _spriteBatch.DrawRectangle(_autoRectangle, Color.White);
            _spriteBatch.End();
        }
    }
}
