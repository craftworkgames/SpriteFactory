using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SpriteFactory.Assets;
using SpriteFactory.MonoGameControls;
using SpriteFactory.Sprites;
using SpriteFactory.Widgets;

namespace SpriteFactory
{
    public class SpriteGroup
    {
        public List<RectangleF> Boxes { get; } = new List<RectangleF>();
    }

    public class MainWindowViewModel : MonoGameViewModel
    {
        public MainWindowViewModel()
        {
        }

        public override void Dispose()
        {
            _assetManager?.Dispose();
            base.Dispose();
        }

        private AssetManager _assetManager;
        //private Vector2 _mousePosition;

        //private Dragger _dragger;
        private SpriteSheetBox _spriteSheetBox;
        //private SpriteBox _focusedSpriteBox;

        //private SelectionTool _selectionTool;
        //private ResizableBox _resizableBox;

        public int Width => GraphicsDevice.Viewport.Width;
        public int Height => GraphicsDevice.Viewport.Height;

        private SpriteEditorViewModel _spriteEditor;
        public SpriteEditorViewModel SpriteEditor
        {
            get => _spriteEditor;
            private set => SetPropertyValue(ref _spriteEditor, value, nameof(SpriteEditor));
        }


        private Cursor _cursor;
        public Cursor Cursor
        {
            get => _cursor;
            set => SetPropertyValue(ref _cursor, value, nameof(Cursor));
        }

        public override void OnMouseDown(MouseStateArgs mouseState)
        {
            //var worldPosition = _camera.ScreenToWorld(mouseState.Position);

            //if (mouseState.LeftButton == ButtonState.Pressed)
            //{
            //    if (_resizableBox == null || !_resizableBox.TryGrab(worldPosition))
            //    {
            //        _selectionTool = new SelectionTool(worldPosition);
            //    }




            //    //var spriteBox = TrySelectSpriteBox(worldPosition);

            //    //if (spriteBox != null)
            //    //{
            //    //    _dragger = new Dragger(spriteBox, worldPosition);
            //    //    _focusedSpriteBox = spriteBox;
            //    //}
            //    //else
            //    //{
            //    //    _focusedSpriteBox = null;

            //    //    if (mouseState.LeftButton == ButtonState.Pressed)
            //    //        _selectionTool = new SelectionTool(worldPosition);
            //    //}

            //}
        }

        //private SpriteBox TrySelectSpriteBox(Vector2 worldPosition)
        //{
        //    if (_spriteSheetBox == null)
        //        return null;

        //    _spriteSheetBox.SpriteBoxes.ForEach(s => s.IsSelected = false);

        //    foreach (var spriteBox in _spriteSheetBox.SpriteBoxes)
        //    {
        //        if (spriteBox.BoundingRectangle.Contains(worldPosition))
        //        {
        //            spriteBox.IsSelected = true;
        //            return spriteBox;
        //        }
        //    }

        //    return null;
        //}

        public override void OnMouseMove(MouseStateArgs mouseState)
        {
            //var worldPosition = _camera.ScreenToWorld(mouseState.Position);
            //var previousWorldPosition = _camera.ScreenToWorld(_mousePosition);
            //var mouseDelta = previousWorldPosition - worldPosition;

            //if (mouseState.MiddleButton == ButtonState.Pressed)
            //    _camera.Move(mouseDelta);

            //if (_resizableBox != null)
            //{
            //    var handle = _resizableBox.GetResizeHandle(worldPosition);
            //    Cursor = GetResizeCursor(handle);
            //}
            //else
            //{
            //    Cursor = null;
            //}

            //if (mouseState.LeftButton == ButtonState.Pressed)
            //{
            //    if (_resizableBox != null && _resizableBox.IsGrabbed)
            //    {
            //        _resizableBox.Resize(worldPosition);
            //    }
            //    else
            //    {
            //        _selectionTool?.OnMouseMove(worldPosition);
            //    }
            //    //_dragger?.OnMouseMove(worldPosition);

            //    //if (handle == ResizeHandle.None)
            //    //    _selectionTool?.OnMouseMove(worldPosition);
            //    //else
            //    //    _resizableBox.Resize(worldPosition);
            //}


            //_mousePosition = mouseState.Position;
        }


        public override void OnMouseUp(MouseStateArgs mouseState)
        {
            //if (mouseState.LeftButton == ButtonState.Released)
            //{
            //    _resizableBox?.Release();

            //    if (_selectionTool != null && !_selectionTool.BoundingRectangle.Size.IsEmpty)
            //    {
            //        _resizableBox = new ResizableBox(_selectionTool.BoundingRectangle.ToRectangle());
            //        _selectionTool = null;
            //    }
            //}
            //if (_selectionTool != null && !_selectionTool.BoundingRectangle.IsEmpty)
            //{
            //    _spriteSheetBox?.SpriteBoxes.Add(new SpriteBox(_selectionTool.BoundingRectangle.ToRectangle()));
            //    _selectionTool = null;
            //}

            //_dragger = null;
        }

        public override void OnMouseWheel(MouseStateArgs args, int delta)
        {
            SpriteEditor.Camera.ZoomIn(delta / 1000f);
            base.OnMouseWheel(args, delta);
        }

        public override void LoadContent()
        {
            //_resizableBox = new ResizableBox(new Rectangle(-100, -100, 200, 300));
            _assetManager = new AssetManager(GraphicsDevice);

            SpriteEditor = new SpriteEditorViewModel(Content, _assetManager, GraphicsDevice)
            {
                OnTextureChanged = texture =>
                {
                    _spriteSheetBox?.Dispose();
                    _spriteSheetBox = new SpriteSheetBox(texture);
                }
            };
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteEditor?.Draw();
        }

        //private static Cursor GetResizeCursor(ResizeHandle handle)
        //{
        //    switch (handle)
        //    {
        //        case ResizeHandle.TopLeft:
        //            return Cursors.SizeNWSE;

        //        case ResizeHandle.TopRight:
        //            return Cursors.SizeNESW;

        //        case ResizeHandle.BottomLeft:
        //            return Cursors.SizeNESW;

        //        case ResizeHandle.BottomRight:
        //            return Cursors.SizeNWSE;

        //        case ResizeHandle.Left:
        //        case ResizeHandle.Right:
        //            return Cursors.SizeWE;

        //        case ResizeHandle.Top:
        //        case ResizeHandle.Bottom:
        //            return Cursors.SizeNS;
        //        case ResizeHandle.Centre:
        //            return Cursors.SizeAll;
        //        case ResizeHandle.None:
        //            return null;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(handle), handle, null);
        //    }
        //}
    }
}