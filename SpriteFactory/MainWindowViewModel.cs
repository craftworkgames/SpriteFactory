using System.IO;
using System.Windows.Input;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SpriteFactory.Json;
using SpriteFactory.MonoGameControls;
using SpriteFactory.Sprites;

namespace SpriteFactory
{
    public class MainWindowViewModel : MonoGameViewModel
    {
        public MainWindowViewModel()
        {
            NewCommand = new Command(New);
            OpenCommand = new Command(Open);
            SaveCommand = new Command(Save);
            SaveAsCommand = new Command(SaveAs);
        }


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

        public ICommand NewCommand { get; }

        public void New()
        {

        }

        private static JsonSerializer CreateJsonSerializer()
        {
            var jsonSerializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters =
                {
                    new StringEnumConverter(),
                    new FlatObservableCollectionIntConverter()
                }
            };
            return jsonSerializer;
        }

        private T OpenFileDialog<T>() where T : IFileSupport
        {
            var service = DependencyResolver.Resolve<T>();
            service.Filter = "Sprites (*.sprites)|*.sprites";
            return service;
        }

        public ICommand OpenCommand { get; }

        public async void Open()
        {
            var openFileService = OpenFileDialog<IOpenFileService>();

            if (await openFileService.DetermineFileAsync())
            {
                var jsonSerializer = CreateJsonSerializer();
                var filePath = openFileService.FileName;

                using (var streamReader = new StreamReader(filePath))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var data = jsonSerializer.Deserialize<SpritesFile>(jsonReader);
                    SpriteEditor.SetData(filePath, data);
                }
            }
        }

        public ICommand SaveCommand { get; }

        public void Save()
        {
            // TODO: Detect if the file has already been saved?
            SaveAs();
        }

        public ICommand SaveAsCommand { get; }

        public async void SaveAs()
        {
            var saveFileService = OpenFileDialog<ISaveFileService>();
            saveFileService.FileName = Path.ChangeExtension(SpriteEditor.TextureName, ".sprites");

            if (await saveFileService.DetermineFileAsync())
            {
                var jsonSerializer = CreateJsonSerializer();
                var filePath = saveFileService.FileName;

                using (var streamWriter = new StreamWriter(filePath))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    var data = SpriteEditor.GetData(filePath);
                    jsonSerializer.Serialize(jsonWriter, data);
                }
            }
        }

        public override void OnMouseDown(MouseStateArgs mouseState)
        {
            SpriteEditor.OnMouseDown(mouseState);
        }

        public override void OnMouseMove(MouseStateArgs mouseState)
        {
            SpriteEditor.OnMouseMove(mouseState);
        }


        public override void OnMouseUp(MouseStateArgs mouseState)
        {
        }

        public override void OnMouseWheel(MouseStateArgs args, int delta)
        {
            SpriteEditor.Camera.ZoomIn(delta / 1000f);
            base.OnMouseWheel(args, delta);
        }

        public override void LoadContent()
        {
            SpriteEditor = new SpriteEditorViewModel(Content,  GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteEditor?.Draw();
        }
    }
}