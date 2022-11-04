using ReactiveUI;
using Avalonia.Controls;
using MessageBox.Avalonia;
using GaSpTK.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;

namespace GaSpTK.Editor
{
    public class MainWindowViewModel : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }
        public Interaction<MainWindowViewModel, string?> ShowOpenDialog { get; }
        public Interaction<MainWindowViewModel, string?> ShowSaveDialog { get; }

        public ReactiveCommand<Unit, Unit> DoNewDocument { get; }
        public ReactiveCommand<Unit, Unit> DoOpenDocument { get; }
        public ReactiveCommand<Unit, Unit> DoSaveDocument { get; }
        public ReactiveCommand<Unit, Unit> DoSaveDocumentAs { get; }

        private File _activeDocument;
        private string? _activeDocumentPath;

        public File ActiveDocument
        {
            get => _activeDocument;
            set => this.RaiseAndSetIfChanged(ref _activeDocument, value);
        }
        public string? ActiveDocumentPath
        {
            get => _activeDocumentPath;
            set => this.RaiseAndSetIfChanged(ref _activeDocumentPath, value);
        }

        public MainWindowViewModel()
        {
            Activator = new ViewModelActivator();
            
            ShowOpenDialog = new Interaction<MainWindowViewModel, string?>();
            ShowSaveDialog = new Interaction<MainWindowViewModel, string?>();

            DoNewDocument = ReactiveCommand.Create(NewDocument);
            DoOpenDocument = ReactiveCommand.CreateFromTask(() =>
            {
                return OpenDocument();
            });
            DoSaveDocument = ReactiveCommand.CreateFromTask(() =>
            {
                return SaveDocument();
            });
            DoSaveDocumentAs = ReactiveCommand.CreateFromTask(() =>
            {
                return SaveDocumentAs();
            });

            _activeDocumentPath = null;
            _activeDocument = new File();
        }

        public void NewDocument()
        {
            ActiveDocument = new File();
            ActiveDocumentPath = null;
        }

        public async Task OpenDocument()
        {
            var path = await ShowOpenDialog.Handle(this);

            if (path != null)
            {
                await OpenDocument(path);
            }
        }

        public async Task OpenDocument(string path)
        {
            try
            {
                string data = await System.IO.File.ReadAllTextAsync(path);
                File? file = JsonConvert.DeserializeObject<File>(data);

                if (file == null)
                {
                    var alertBox = MessageBoxManager.GetMessageBoxStandardWindow("Corrupt/Invalid File", "Unknown error while reading file");
                    await alertBox.Show();
                    return;
                }

                ActiveDocumentPath = path;
                ActiveDocument = file;
            }
            catch (System.IO.IOException e)
            {
                var alertBox = MessageBoxManager.GetMessageBoxStandardWindow("Error", "Failed to open file: " + e.Message);
                await alertBox.Show();
            }
            catch (Newtonsoft.Json.JsonException e)
            {
                var alertBox = MessageBoxManager.GetMessageBoxStandardWindow("Corrupt/Invalid File", e.Message);
                await alertBox.Show();
            }
        }

        public async Task SaveDocument(string path)
        {
            // serialize active document
            try
            {
                string data = JsonConvert.SerializeObject(ActiveDocument);
                await System.IO.File.WriteAllTextAsync(path, data);
                ActiveDocumentPath = path;
            }
            catch (System.IO.IOException e)
            {
                var alertBox = MessageBoxManager.GetMessageBoxStandardWindow("Error", "Failed to save file: " + e.Message);
                await alertBox.Show();
            }
        }

        public async Task SaveDocument()
        {
            if (ActiveDocumentPath != null)
            {
                await SaveDocument(ActiveDocumentPath);
            }
            else
            {
                await SaveDocumentAs();
            }
        }

        public async Task SaveDocumentAs()
        {
            var savePath = await ShowSaveDialog.Handle(this);

            if (savePath != null)
            {
                await SaveDocument(savePath);
            }
        }
    }
}