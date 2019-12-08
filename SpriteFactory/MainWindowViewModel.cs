using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using SpriteFactory.About;
using SpriteFactory.Documents;
using SpriteFactory.Sprites;

namespace SpriteFactory
{
    public class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel()
        {
            NewCommand = new TaskCommand(New);
            OpenCommand = new Command(Open);
            SaveCommand = new Command(Save);
            SaveAsCommand = new Command(SaveAs);

            // ReSharper disable once VirtualMemberCallInConstructor
            Title = App.Name;

            SpriteEditor = new SpriteEditorViewModel();
            SpriteEditor.PropertyChanged += (sender, args) =>
            {
                Document.IsSaved = false;
                UpdateTitle();
            };

            AboutCommand = new Command(About);
        }

        public ICommand AboutCommand { get; }

        private async void About()
        {
            var uiService = DependencyResolver.Resolve<IUIVisualizerService>();
            await uiService.ShowDialogAsync<AboutWindowViewModel>();
        }

        protected override async Task InitializeAsync()
        {
            await New();
        }

        private Document<SpriteFactoryFile> _document;
        public Document<SpriteFactoryFile> Document
        {
            get => _document;
            private set
            {
                if (SetPropertyValue(ref _document, value, nameof(Document)))
                    UpdateTitle();
            }
        }

        private SpriteEditorViewModel _spriteEditor;
        public SpriteEditorViewModel SpriteEditor
        {
            get => _spriteEditor;
            private set => SetPropertyValue(ref _spriteEditor, value, nameof(SpriteEditor));
        }

        private void UpdateTitle()
        {
            var fileName = Document.IsNew ? "(untitled)" : Path.GetFileName(Document.FullPath);
            var savedIndicator = !Document.IsSaved ? "*" : string.Empty;
            Title = $"{App.Name} - {fileName}{savedIndicator}";
        }

        public ICommand NewCommand { get; }

        public async Task New()
        {
            if (await ConfirmSave())
            {
                Document = Document<SpriteFactoryFile>.New();
                SpriteEditor.SetDocumentContent(Document);
                Document.IsSaved = true;
                UpdateTitle();
            }
        }

        private T OpenFileDialog<T>() where T : IFileSupport
        {
            var service = DependencyResolver.Resolve<T>();
            service.Filter = $"{App.Name} (*{App.FileExtension})|*{App.FileExtension}";
            return service;
        }

        public ICommand OpenCommand { get; }
        public async void Open()
        {
            if (await ConfirmSave())
            {
                var openFileService = OpenFileDialog<IOpenFileService>();

                if (await openFileService.DetermineFileAsync())
                {
                    var filePath = openFileService.FileName;
                    Document = Document<SpriteFactoryFile>.Load(filePath);
                    SpriteEditor.SetDocumentContent(Document);
                    Document.IsSaved = true;
                    UpdateTitle();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public void Save()
        {
            if (Document.IsNew)
            {
                SaveAs();
            }
            else
            {
                Document.Save(SpriteEditor.GetDocumentContent);
                UpdateTitle();
            }
        }

        public ICommand SaveAsCommand { get; }

        public async void SaveAs()
        {
            var saveFileService = OpenFileDialog<ISaveFileService>();
            saveFileService.FileName = Path.ChangeExtension(SpriteEditor.TextureName, App.FileExtension);

            if (await saveFileService.DetermineFileAsync())
            {
                var filePath = saveFileService.FileName;
                Document.Save(filePath, SpriteEditor.GetDocumentContent);
                UpdateTitle();
            }
        }

        public async Task<bool> ConfirmSave()
        {
            if (Document != null && !Document.IsSaved)
            {
                var messageService = DependencyResolver.Resolve<IMessageService>();
                var result = await messageService.ShowAsync("Save changes?", "Save", MessageButton.YesNoCancel, MessageImage.Question);

                switch (result)
                {
                    case MessageResult.Yes:
                        Save();
                        return true;
                    case MessageResult.Cancel:
                        return false;
                    default:
                        return true;
                }
            }
            return true;
        }
    }
}