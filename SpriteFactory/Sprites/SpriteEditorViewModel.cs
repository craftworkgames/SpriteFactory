using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Catel.Collections;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SpriteFactory.Documents;
using SpriteFactory.MonoGameControls;

namespace SpriteFactory.Sprites
{
    public class SpriteEditorViewModel : MonoGameViewModel
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _backgroundTexture;
        private SpriteFont _spriteFont;

        public event EventHandler ContentLoaded;

        public SpriteEditorViewModel()
        {
            SelectTextureCommand = new Command(SelectTexture);

            AddAnimationCommand = new Command(AddAnimation);
            RemoveAnimationCommand = new Command(RemoveAnimation, () => SelectedAnimation != null);
            MoveAnimationUpCommand = new Command(() => MoveAnimation(-1), () => SelectedAnimation != null);
            MoveAnimationDownCommand = new Command(() => MoveAnimation(1), () => SelectedAnimation != null);

            SelectedPreviewZoom = PreviewZoomOptions.LastOrDefault();

            IsPlaying = true;

            GoToFirstFrameCommand = new Command(() =>
            {
                if (SelectedAnimation != null)
                {
                    IsPlaying = false;
                    SelectedAnimation.SelectedKeyFrame = SelectedAnimation.KeyFrames.FirstOrDefault();
                }
            });
            BackOneFrameCommand = new Command(() => IncrementKeyFrameIndex(-1));
            PlayCommand = new Command(() => IsPlaying = !IsPlaying);
            ForwardOneFrameCommand = new Command(() => IncrementKeyFrameIndex(1));
            GoToLastFrameCommand = new Command(() =>
            {
                if (SelectedAnimation != null)
                {
                    IsPlaying = false;
                    SelectedAnimation.SelectedKeyFrame = SelectedAnimation.KeyFrames.LastOrDefault();
                }
            });

            AddSelectedFramesCommand = new Command(AddSelectedKeyFrames);
            MoveFrameLeftCommand = new Command(() => MoveFrame(-1));
            MoveFrameRightCommand = new Command(() => MoveFrame(1));
            DuplicateFrameCommand = new Command(DuplicateFrame);
            DeleteFrameCommand = new Command(DeleteFrame);
        }

        private void DeleteFrame()
        {
            var animation = SelectedAnimation;
            var frame = animation?.SelectedKeyFrame;

            if (frame != null)
            {
                var index = animation.KeyFrames.IndexOf(frame);
                animation.KeyFrames.Remove(frame);
                animation.SelectedKeyFrame = index < animation.KeyFrames.Count ? animation.KeyFrames[index] : animation.KeyFrames.LastOrDefault();
            }
        }

        private void DuplicateFrame()
        {
            var animation = SelectedAnimation;
            var frame = animation?.SelectedKeyFrame;
            
            if (frame != null)
            {
                var index = animation.KeyFrames.IndexOf(frame);
                animation.KeyFrames.Insert(index, new KeyFrameViewModel(frame.Index, () => TexturePath, GetFrameRectangle));
            }
        }

        private void MoveFrame(int increment)
        {
            var animation = SelectedAnimation;
            var frame = animation?.SelectedKeyFrame;

            if (frame != null)
            {
                var index = animation.KeyFrames.IndexOf(frame);
                var newIndex = index + increment;

                if (newIndex >= 0 && newIndex < animation.KeyFrames.Count)
                {
                    animation.KeyFrames.Remove(frame);
                    animation.KeyFrames.Insert(newIndex, new KeyFrameViewModel(frame.Index, () => TexturePath, GetFrameRectangle));
                    animation.SelectedKeyFrame = animation.KeyFrames[newIndex];
                }
            }
        }

        private void MoveAnimation(int increment)
        {
            var selectedAnimation = SelectedAnimation;
            var index = Animations.IndexOf(selectedAnimation);
            var newIndex = index + increment;

            if(newIndex < 0 || newIndex >= Animations.Count)
                return;

            Animations.RemoveAt(index);
            Animations.Insert(newIndex, selectedAnimation);
            SelectedAnimation = selectedAnimation;
        }

        private void IncrementKeyFrameIndex(int increment)
        {
            if (SelectedAnimation?.SelectedKeyFrame != null)
            {
                var index = SelectedAnimation.KeyFrames.IndexOf(SelectedAnimation.SelectedKeyFrame) + increment;

                if (index >= SelectedAnimation.KeyFrames.Count)
                    index = 0;
                else if (index < 0)
                    index = SelectedAnimation.KeyFrames.Count - 1;

                SelectedAnimation.SelectedKeyFrame = SelectedAnimation.KeyFrames[index];
                IsPlaying = false;
            }
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundTexture = Content.Load<Texture2D>("checkered-dark");
            _spriteFont = Content.Load<SpriteFont>("default");

            Camera = new OrthographicCamera(GraphicsDevice);
            Camera.LookAt(Vector2.Zero);

            ContentLoaded?.Invoke(this, EventArgs.Empty);
        }

        public int Width => GraphicsDevice.Viewport.Width;
        public int Height => GraphicsDevice.Viewport.Height;

        public OrthographicCamera Camera { get; private set; }

        private Cursor _cursor;
        public Cursor Cursor
        {
            get => _cursor;
            set => SetPropertyValue(ref _cursor, value, nameof(Cursor));
        }

        public ZoomOptionViewModel[] PreviewZoomOptions { get; } =
        {
            new ZoomOptionViewModel(1),
            new ZoomOptionViewModel(2),
            new ZoomOptionViewModel(4),
            new ZoomOptionViewModel(8)
        };

        private ZoomOptionViewModel _selectedPreviewZoom;
        public ZoomOptionViewModel SelectedPreviewZoom
        {
            get => _selectedPreviewZoom;
            set => SetPropertyValue(ref _selectedPreviewZoom, value, nameof(SelectedPreviewZoom));
        }

        public Vector2 Origin => Texture != null ? new Vector2(Texture.Width / 2f, Texture.Height / 2f) : Vector2.Zero;
        public Rectangle TextureBounds => Texture?.Bounds ?? Rectangle.Empty;

        private string _texturePath;
        public string TexturePath
        {
            get => _texturePath;
            private set
            {
                if (SetPropertyValue(ref _texturePath, value, nameof(TexturePath)))
                {
                    TextureName = Path.GetFileName(_texturePath);
                    Texture = _texturePath != null ? Content.LoadRaw<Texture2D>(_texturePath) : null;
                    Animations.Clear();
                    SelectedAnimation = null;
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
                {
                    if(_texture != null)
                        Camera.LookAt(_texture.Bounds.Center.ToVector2());
                }
            }
        }
        
        public ICommand SelectTextureCommand { get; }

        private int _tileWidth = 32;
        public int TileWidth
        {
            get => _tileWidth;
            set
            {
                if (SetPropertyValue(ref _tileWidth, value, nameof(TileWidth)))
                    SelectedAnimation = null;
            }
        }

        private int _tileHeight = 32;
        public int TileHeight
        {
            get => _tileHeight;
            set
            {
                if (SetPropertyValue(ref _tileHeight, value, nameof(TileHeight)))
                    SelectedAnimation = null;
            }
        }

        public Vector2 WorldPosition { get; set; }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            private set => SetPropertyValue(ref _isPlaying, value, nameof(IsPlaying));
        }

        public ObservableCollection<KeyFrameAnimationViewModel> Animations { get; } = new ObservableCollection<KeyFrameAnimationViewModel>();

        private KeyFrameAnimationViewModel _selectedAnimation;
        public KeyFrameAnimationViewModel SelectedAnimation
        {
            get => _selectedAnimation;
            set => SetPropertyValue(ref _selectedAnimation, value, nameof(SelectedAnimation));
        }

        public ObservableCollection<KeyFrameViewModel> SelectedKeyFrames { get; } = new ObservableCollection<KeyFrameViewModel>();

        public ICommand AddAnimationCommand { get; }
        public ICommand RemoveAnimationCommand { get; }
        public ICommand MoveAnimationUpCommand { get; }
        public ICommand MoveAnimationDownCommand { get; }

        public ICommand GoToFirstFrameCommand { get; }
        public ICommand BackOneFrameCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand ForwardOneFrameCommand { get; }
        public ICommand GoToLastFrameCommand { get; }

        public ICommand AddSelectedFramesCommand { get; }
        public ICommand MoveFrameLeftCommand { get; }
        public ICommand MoveFrameRightCommand { get; }
        public ICommand DuplicateFrameCommand { get; }
        public ICommand DeleteFrameCommand { get; }
        
        private void AddSelectedKeyFrames()
        {
            if (SelectedAnimation != null && SelectedKeyFrames.Any())
            {
                SelectedAnimation.KeyFrames.AddRange(SelectedKeyFrames);
                SelectedKeyFrames.Clear();
            }
        }

        private void AddAnimation()
        {
            var animation = new KeyFrameAnimationViewModel {Name = $"animation{Animations.Count}"};
            Animations.Add(animation);
            SelectedAnimation = animation;
            AddSelectedKeyFrames();
        }

        private void RemoveAnimation()
        {
            if (SelectedAnimation != null)
            {
                var index = Animations.IndexOf(SelectedAnimation);
                Animations.Remove(SelectedAnimation);
                SelectedAnimation = index >= Animations.Count ? Animations.LastOrDefault() : Animations[index];
            }
        }

        private async void SelectTexture()
        {
            var openFileService = DependencyResolver.Resolve<IOpenFileService>();
            openFileService.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";

            if (await openFileService.DetermineFileAsync())
                TexturePath = openFileService.FileName;
        }

        private Vector2 _previousMousePosition;
        
        public override void OnMouseDown(MouseStateArgs mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                SelectedKeyFrames.Clear();
                var frameIndex = GetFrameIndex();

                if (frameIndex.HasValue)
                {
                    var index = frameIndex.Value;
                    var keyFrame = new KeyFrameViewModel(index, () => TexturePath, GetFrameRectangle);
                    SelectedKeyFrames.Add(keyFrame);
                }
            }
        }

        public override void OnMouseWheel(MouseStateArgs args, int delta)
        {
            Camera.ZoomIn(delta / 1000f);
        }

        private int? GetFrameIndex()
        {
            if (Texture == null || !Texture.Bounds.Contains(WorldPosition))
                return null;

            var columns = Texture.Width / TileWidth;
            var cx = (int)(WorldPosition.X / TileWidth);
            var cy = (int)(WorldPosition.Y / TileHeight);
            var frameIndex = cy * columns + cx;

            return frameIndex;
        }

        private Rectangle GetFrameRectangle(int frame)
        {
            var columns = Texture.Width / TileWidth;
            var cy = frame / columns;
            var cx = frame - cy * columns;
            var rectangle = new Rectangle(cx * TileWidth, cy * TileHeight, TileWidth, TileHeight);

            if (rectangle.Right > TextureBounds.Right)
                return Rectangle.Empty;

            if(rectangle.Bottom > TextureBounds.Bottom)
                return Rectangle.Empty;

            return rectangle;
        }

        public override void OnMouseMove(MouseStateArgs mouseState)
        {
            WorldPosition = Camera.ScreenToWorld(mouseState.Position);
            
            var previousWorldPosition = Camera.ScreenToWorld(_previousMousePosition);
            var mouseDelta = previousWorldPosition - WorldPosition;

            if (mouseState.RightButton == ButtonState.Pressed)
                Camera.Move(mouseDelta);

            if (mouseState.LeftButton == ButtonState.Pressed) // && SelectedAnimation != null)
            {
                var frameIndex = GetFrameIndex();

                if (frameIndex.HasValue && SelectedKeyFrames.All(k => k.Index != frameIndex.Value))
                {
                    var keyFrame = new KeyFrameViewModel(frameIndex.Value, () => TexturePath, GetFrameRectangle);
                    SelectedKeyFrames.Add(keyFrame);
                }
            }

            _previousMousePosition = mouseState.Position;
        }

        private int _frameIndex;
        private float _nextFrameHackCounter;
        
        private Rectangle GetPreviewRectangle()
        {
            if(TileWidth == 0 || TileHeight == 0)
                return Rectangle.Empty;

            const int max = 256;
            var previewZoom = SelectedPreviewZoom.Value;
            var width = TileWidth * previewZoom;
            var height = TileHeight * previewZoom;
            var ratio = TileWidth / (float) TileHeight;

            if (width > max || height > max)
            {
                if (ratio >= 1f)
                {
                    width = max;
                    height = (int) (max / ratio);
                }
                else if (height > max)
                {
                    height = max;
                    width = (int) (max * ratio);
                }
            }

            var x = GraphicsDevice.Viewport.Width - width;
            return new Rectangle(x, 0, width, height);
        }

        public SpriteFactoryFile GetDocumentContent(Document<SpriteFactoryFile> document)
        {
            return new SpriteFactoryFile
            {
                TextureAtlas = new TextureAtlas
                {
                    Texture = document.GetRelativePath(TexturePath),
                    RegionWidth = TileWidth,
                    RegionHeight = TileHeight
                },
                Cycles = Animations.ToDictionary(a => a.Name, a => a.ToAnimationCycle())
            };
        }

        public void SetDocumentContent(Document<SpriteFactoryFile> document)
        {
            var data = document.Content;
            TexturePath = document.IsNew ? null : document.GetFullPath(data.TextureAtlas.Texture);
            TileWidth = data.TextureAtlas.RegionWidth;
            TileHeight = data.TextureAtlas.RegionHeight;
            Animations.Clear();

            foreach (var keyValuePair in data.Cycles)
            {
                var name = keyValuePair.Key;
                var animation = keyValuePair.Value;
                Animations.Add(KeyFrameAnimationViewModel.FromAnimation(name, animation, () => TexturePath, GetFrameRectangle));
            }
            SelectedAnimation = Animations.FirstOrDefault();
        }

        private KeyFrameViewModel GetCurrentFrame()
        {
            if (SelectedAnimation.SelectedKeyFrame == null)
                return SelectedAnimation.KeyFrames.FirstOrDefault();

            return SelectedAnimation.SelectedKeyFrame;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (IsPlaying && SelectedAnimation?.KeyFrames.Count > 0)
            {
                _nextFrameHackCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_nextFrameHackCounter >= SelectedAnimation.FrameDuration)
                {
                    _frameIndex++;
                    _nextFrameHackCounter = 0;
                }

                if (_frameIndex >= SelectedAnimation.KeyFrames.Count)
                    _frameIndex = 0;

                var frame = SelectedAnimation.KeyFrames[_frameIndex];
                SelectedAnimation.SelectedKeyFrame = frame;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (Texture == null)
                return;

            // main texture
            var boundingRectangle = TextureBounds;

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointWrap, transformMatrix: Camera.GetViewMatrix());
            _spriteBatch.Draw(_backgroundTexture, sourceRectangle: boundingRectangle, destinationRectangle: boundingRectangle, color: Color.White);

            foreach (var keyFrame in SelectedKeyFrames)
            {
                var keyFrameRectangle = GetFrameRectangle(keyFrame.Index);
                _spriteBatch.FillRectangle(keyFrameRectangle, Color.CornflowerBlue * 0.5f);
            }

            if (SelectedAnimation != null)
            {
                foreach (var keyFrame in SelectedAnimation.KeyFrames)
                {
                    var keyFrameRectangle = GetFrameRectangle(keyFrame.Index);
                    _spriteBatch.FillRectangle(keyFrameRectangle, Color.Gray * 0.25f);
                }
            }

            _spriteBatch.Draw(Texture, sourceRectangle: boundingRectangle, destinationRectangle: boundingRectangle, color: Color.White);

            // highlighter
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

            // animation preview
            if (SelectedAnimation != null && SelectedAnimation.KeyFrames.Any())
            {
                var frame = GetCurrentFrame();
                var sourceRectangle = GetFrameRectangle(frame.Index);
                var previewRectangle = GetPreviewRectangle();

                _spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
                _spriteBatch.Draw(_backgroundTexture, previewRectangle, null, Color.White);
                _spriteBatch.DrawRectangle(previewRectangle, Color.White * 0.5f);
                _spriteBatch.Draw(Texture, previewRectangle, sourceRectangle, Color.White);
                _spriteBatch.End();
            }

            // debug text
            var frameIndex = GetFrameIndex();

            if (frameIndex.HasValue)
            {
                var frameRectangle = GetFrameRectangle(frameIndex.Value);
                _spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
                _spriteBatch.DrawString(_spriteFont, $"frame: {frameIndex} ({frameRectangle.X}, {frameRectangle.Y})", Vector2.Zero, Color.White);
                _spriteBatch.End();
            }
        }
    }
}
