using System.IO;
using System.Windows.Input;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using SpriteFactory.Documents;
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

            SpriteEditor = new SpriteEditorViewModel();
        }

        public Document<SpriteFactoryFile> Document { get; set; }
        
        private SpriteEditorViewModel _spriteEditor;
        public SpriteEditorViewModel SpriteEditor
        {
            get => _spriteEditor;
            private set => SetPropertyValue(ref _spriteEditor, value, nameof(SpriteEditor));
        }
        
        public ICommand NewCommand { get; }

        public void New()
        {
            var context = new DocumentContext();
            Document = new Document<SpriteFactoryFile>();
            SpriteEditor.SetDocumentContent(context, Document.Content);
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
                var filePath = openFileService.FileName;
                var context = new DocumentContext(filePath);

                Document = Document<SpriteFactoryFile>.Load(filePath);
                SpriteEditor.SetDocumentContent(context, Document.Content);
            }
        }

        public ICommand SaveCommand { get; }
        public void Save()
        {
            if (!Document.IsSaved)
            {
                SaveAs();
            }
            else
            {
                var filePath = Document.FullPath;
                SaveDocument(filePath);
            }
        }

        public ICommand SaveAsCommand { get; }
        public async void SaveAs()
        {
            var saveFileService = OpenFileDialog<ISaveFileService>();
            saveFileService.FileName = Path.ChangeExtension(SpriteEditor.TextureName, ".sprites");

            if (await saveFileService.DetermineFileAsync())
            {
                var filePath = saveFileService.FileName;
                SaveDocument(filePath);
            }
        }

        private void SaveDocument(string filePath)
        {
            var documentContext = new DocumentContext(filePath);
            var content = SpriteEditor.GetDocumentContent(documentContext);
            Document.Save(filePath, content);
        }
    }


}