using System;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteFactory.MonoGameControls
{
    public interface IMonoGameViewModel : IDisposable
    {
        IGraphicsDeviceService GraphicsDeviceService { get; set; }

        void Initialize();
        void LoadContent();
        void UnloadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void OnActivated(object sender, EventArgs args);
        void OnDeactivated(object sender, EventArgs args);
        void OnExiting(object sender, EventArgs args);

        void SizeChanged(object sender, SizeChangedEventArgs args);

        void OnMouseDown(MouseStateArgs mouseState);
        void OnMouseMove(MouseStateArgs mouseState);
        void OnMouseUp(MouseStateArgs mouseState);

        void OnDrop(DragStateArgs dragState);
        void OnMouseWheel(MouseStateArgs args, int delta);
        void OnSizeChanged(int width, int height);
    }

    public class MonoGameViewModel : ViewModel, IMonoGameViewModel
    {
        public MonoGameViewModel()
        {
        }

        public virtual void Dispose()
        {
            Content?.Dispose();
        }

        public IGraphicsDeviceService GraphicsDeviceService { get; set; }
        protected GraphicsDevice GraphicsDevice => GraphicsDeviceService?.GraphicsDevice;
        protected MonoGameServiceProvider Services { get; private set; }
        protected ContentManagerExtended Content { get; set; }

        public virtual void Initialize()
        {
            Services = new MonoGameServiceProvider();
            Services.AddService(GraphicsDeviceService);
            Content = new ContentManagerExtended(Services);
        }

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void OnActivated(object sender, EventArgs args) { }
        public virtual void OnDeactivated(object sender, EventArgs args) { }
        public virtual void OnExiting(object sender, EventArgs args) { }
        public virtual void SizeChanged(object sender, SizeChangedEventArgs args) { }
        public virtual void OnMouseDown(MouseStateArgs mouseState) { }
        public virtual void OnMouseMove(MouseStateArgs mouseState) { }
        public virtual void OnMouseUp(MouseStateArgs mouseState) { }
        public virtual void OnMouseWheel(MouseStateArgs args, int delta) { }
        public virtual void OnDrop(DragStateArgs dragState) { }
        public virtual void OnSizeChanged(int width, int height) { }
    }
}
