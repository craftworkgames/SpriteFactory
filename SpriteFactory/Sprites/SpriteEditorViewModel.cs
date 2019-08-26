using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SpriteFactory.Assets;
using SpriteFactory.MonoGameControls;

namespace SpriteFactory.Sprites
{
    public class SpriteEditorViewModel : ViewModel
    {
        private readonly AssetManager _assetManager;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _backgroundTexture;
        
        public SpriteEditorViewModel(ContentManager contentManager, AssetManager assetManager, GraphicsDevice graphicsDevice)
        {
            _assetManager = assetManager;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            _backgroundTexture = contentManager.Load<Texture2D>("checkered-dark");

            Camera = new OrthographicCamera(graphicsDevice);
            Camera.LookAt(Vector2.Zero);

            SelectTextureCommand = new Command(SelectTexture);

            AddAnimationCommand = new Command(AddAnimation);
            RemoveAnimationCommand = new Command(RemoveAnimation, () => SelectedAnimation != null);
        }

        public OrthographicCamera Camera { get; }

        public Vector2 Origin => Texture != null ? new Vector2(Texture.Width / 2f, Texture.Height / 2f) : Vector2.Zero;
        public Rectangle BoundingRectangle => Texture?.Bounds ?? Rectangle.Empty;

        private string _texturePath;
        public string TexturePath
        {
            get => _texturePath;
            private set
            {
                if (SetPropertyValue(ref _texturePath, value, nameof(TexturePath)))
                {
                    TextureName = Path.GetFileName(_texturePath);
                    Texture = _assetManager.LoadTexture(TexturePath);
                }
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
                if (SetPropertyValue(ref _texture, value, nameof(Texture)))
                    Camera.LookAt(Texture.Bounds.Center.ToVector2());
            }
        }
        
        public ICommand SelectTextureCommand { get; }

        private int _tileWidth = 32;
        public int TileWidth
        {
            get => _tileWidth;
            set => SetPropertyValue(ref _tileWidth, value, nameof(TileWidth));
        }

        private int _tileHeight = 32;
        public int TileHeight
        {
            get => _tileHeight;
            set => SetPropertyValue(ref _tileHeight, value, nameof(TileHeight));
        }

        public Vector2 WorldPosition { get; set; }

        public ObservableCollection<SpriteKeyFrameAnimation> Animations { get; } = new ObservableCollection<SpriteKeyFrameAnimation>();

        private SpriteKeyFrameAnimation _selectedAnimation;
        public SpriteKeyFrameAnimation SelectedAnimation
        {
            get => _selectedAnimation;
            set => SetPropertyValue(ref _selectedAnimation, value, nameof(SelectedAnimation));
        }

        public ICommand AddAnimationCommand { get; }
        public ICommand RemoveAnimationCommand { get; }

        private void AddAnimation()
        {
            var animation = new SpriteKeyFrameAnimation {Name = $"animation{Animations.Count}"};
            Animations.Add(animation);
            SelectedAnimation = animation;
        }

        private void RemoveAnimation()
        {
            if (SelectedAnimation != null)
                Animations.Remove(SelectedAnimation);
        }

        private async void SelectTexture()
        {
            var openFileService = DependencyResolver.Resolve<IOpenFileService>();
            openFileService.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";

            if (await openFileService.DetermineFileAsync())
                TexturePath = openFileService.FileName;
        }

        //private void AutoDetect()
        //{
        //    var data = new Color[Texture.Width * Texture.Height];
        //    Texture.GetData(data);

        //    for (var y = 0; y < Texture.Height; y++)
        //    {
        //        for (var x = 0; x < Texture.Width; x++)
        //        {
        //            var color = data[y * Texture.Height + x];

        //            if (color.A != 0)
        //            {
        //                if(_autoRectangle.IsEmpty)
        //                    _autoRectangle = new Rectangle(x, y, 1, 1);
        //            }
        //        }
        //    }
        //}

        private Vector2 _previousMousePosition;
        
        public void OnMouseDown(MouseStateArgs mouseState)
        {
            var columns = Texture.Width / TileWidth;
            var cx = (int)(WorldPosition.X / TileWidth);
            var cy = (int)(WorldPosition.Y / TileHeight);
            var frameIndex = cy * columns + cx;

            SelectedAnimation.KeyFrames.Add(frameIndex);
        }

        public void OnMouseMove(MouseStateArgs mouseState)
        {
            WorldPosition = Camera.ScreenToWorld(mouseState.Position);

            var previousWorldPosition = Camera.ScreenToWorld(_previousMousePosition);
            var mouseDelta = previousWorldPosition - WorldPosition;

            if (mouseState.RightButton == ButtonState.Pressed)
                Camera.Move(mouseDelta);

            _previousMousePosition = mouseState.Position;
        }

        private int _frameIndex;
        private int _nextFrameHackCounter;

        public void Draw()
        {
            if(Texture == null)
                return;

            var boundingRectangle = BoundingRectangle;
            
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointWrap, transformMatrix: Camera.GetViewMatrix());
            _spriteBatch.Draw(_backgroundTexture, sourceRectangle: boundingRectangle, destinationRectangle: boundingRectangle, color: Color.White);
            _spriteBatch.Draw(Texture, sourceRectangle: boundingRectangle, destinationRectangle: boundingRectangle, color: Color.White);

            if (TileWidth > 1 && TileHeight > 1)
            {
                for (var y = 0; y <= Texture.Height; y += TileHeight)
                    _spriteBatch.DrawLine(0, y, boundingRectangle.Width, y, Color.White * 0.5f);

                for (var x = 0; x <= Texture.Width; x += TileWidth)
                    _spriteBatch.DrawLine(x, 0, x, boundingRectangle.Height, Color.White * 0.5f);

                if (boundingRectangle.Contains(WorldPosition))
                {
                    var cx = (int)(WorldPosition.X / TileWidth);
                    var cy = (int)(WorldPosition.Y / TileHeight);

                    _spriteBatch.FillRectangle(cx * TileWidth, cy * TileHeight, TileWidth, TileHeight, Color.CornflowerBlue * 0.5f);
                }
            }
            
            _spriteBatch.End();

            if (SelectedAnimation != null)
            {
                if (SelectedAnimation.KeyFrames.Any())
                {
                    _nextFrameHackCounter++;

                    if (_nextFrameHackCounter >= 10)
                    {
                        _frameIndex++;
                        _nextFrameHackCounter = 0;
                    }

                    if (_frameIndex >= SelectedAnimation.KeyFrames.Count)
                        _frameIndex = 0;

                    var frame = SelectedAnimation.KeyFrames[_frameIndex];
                    var sourceRectangle = new Rectangle(frame * TileWidth, 0, TileWidth, TileHeight);
                    var destinationRectangle = new Rectangle(0, 0, TileWidth, TileHeight);

                    _spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointWrap, transformMatrix: Matrix.CreateScale(8));
                    _spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
                    _spriteBatch.End();
                }
            }

        }

        public SpritesFile GetData(string filePath)
        {
            return new SpritesFile
            {
                Texture = Catel.IO.Path.GetRelativePath(TexturePath, Path.GetDirectoryName(filePath)),
                Mode = SpriteMode.Tileset,
                Content = new TilesetContent
                {
                    TileWidth = TileWidth,
                    TileHeight = TileHeight
                }
            };
        }

        public void SetData(string filePath, SpritesFile data)
        {
            var directory = Path.GetDirectoryName(filePath);
            // ReSharper disable once AssignNullToNotNullAttribute
            var texturePath = Path.Combine(directory, data.Texture);

            TexturePath = texturePath;
            TileWidth = data.Content.TileWidth;
            TileHeight = data.Content.TileHeight;
        }
    }
}
