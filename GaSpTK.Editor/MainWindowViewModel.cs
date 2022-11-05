using ReactiveUI;
using Avalonia.Controls;
using MessageBox.Avalonia;
using GaSpTK.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Interaction<MainWindowViewModel, string?> ShowOpenImageDialog { get; }

        public ReactiveCommand<Unit, Unit> DoNewDocument { get; }
        public ReactiveCommand<Unit, Unit> DoOpenDocument { get; }
        public ReactiveCommand<Unit, Unit> DoSaveDocument { get; }
        public ReactiveCommand<Unit, Unit> DoSaveDocumentAs { get; }
        public ReactiveCommand<Unit, Unit> DoNewAtlas { get; }
        public ReactiveCommand<SpriteAtlas, Unit> DoDeleteAtlas { get; }

        private File _activeDocument;
        private string? _activeDocumentPath;
        private bool _unsaved;

        public File ActiveDocument
        {
            get => _activeDocument;
            set
            {
                this.RaiseAndSetIfChanged(ref _activeDocument, value);
                this.RaisePropertyChanged("WindowTitle");
            }
        }
        public string? ActiveDocumentPath
        {
            get => _activeDocumentPath;
            set
            {
                this.RaiseAndSetIfChanged(ref _activeDocumentPath, value);
                this.RaisePropertyChanged("WindowTitle");
            }
        }
        public bool Unsaved
        {
            get => _unsaved;
            set
            {
                this.RaiseAndSetIfChanged(ref _unsaved, value);
                this.RaisePropertyChanged("WindowTitle");
            }
        }
        public ObservableCollection<SpriteAtlas> Atlas { get; }
        public string WindowTitle => $"GaSpTK Editor - {_activeDocumentPath ?? "Untitled Document"} {(_unsaved ? "*" : "")}";
        public string RootPath => System.IO.Path.GetDirectoryName(ActiveDocumentPath)!;

        public MainWindowViewModel()
        {
            Activator = new ViewModelActivator();
            
            ShowOpenDialog = new Interaction<MainWindowViewModel, string?>();
            ShowSaveDialog = new Interaction<MainWindowViewModel, string?>();
            ShowOpenImageDialog = new Interaction<MainWindowViewModel, string?>();

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
            DoNewAtlas = ReactiveCommand.CreateFromTask(() =>
            {
                return NewAtlas();
            });
            DoDeleteAtlas = ReactiveCommand.CreateFromTask((SpriteAtlas atlas) =>
            {
                return DeleteAtlas(atlas);
            });

            Atlas = new ObservableCollection<SpriteAtlas>();

            _activeDocumentPath = null;
            _activeDocument = new File();
            _unsaved = true;
        }

        public void NewDocument()
        {
            ActiveDocument = new File();
            ActiveDocumentPath = null;
            Atlas.Clear();
            Unsaved = true;
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
                Unsaved = false;

                Atlas.Clear();
                foreach (var atlas in file.Atlas)
                {
                    Atlas.Add(atlas);
                }
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
                ActiveDocument.Atlas.Clear();
                foreach (var atlas in Atlas)
                {
                    ActiveDocument.Atlas.Add(atlas);
                }

                string data = JsonConvert.SerializeObject(ActiveDocument);
                await System.IO.File.WriteAllTextAsync(path, data);
                ActiveDocumentPath = path;
                Unsaved = false;
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

        public async Task NewAtlas()
        {
            // NOTE: in order to create a new atlas, current document must be saved
            if (ActiveDocumentPath == null)
            {
                var alertBox = MessageBoxManager.GetMessageBoxStandardWindow("Save Document", "Document must be saved in order to continue", MessageBox.Avalonia.Enums.ButtonEnum.OkCancel,
                    MessageBox.Avalonia.Enums.Icon.Info);
                var buttonResult = await alertBox.Show();

                if (buttonResult == MessageBox.Avalonia.Enums.ButtonResult.Cancel)
                {
                    return;
                }
                
                await SaveDocumentAs();

                if (ActiveDocumentPath == null)
                {
                    return;
                }
            }

            var imgPath = await ShowOpenImageDialog.Handle(this);

            if (imgPath != null)
            {
                var atlas = new SpriteAtlas();
                var rootpath = System.IO.Path.GetDirectoryName(ActiveDocumentPath) ?? throw new System.InvalidOperationException("Invalid document path");
                atlas.Id = System.IO.Path.GetFileNameWithoutExtension(imgPath);
                atlas.Path = System.IO.Path.GetRelativePath(rootpath, imgPath);

                Atlas.Add(atlas);
                Unsaved = true;
            }
        }

        public async Task DeleteAtlas(SpriteAtlas atlas)
        {
            var alertBox = MessageBoxManager.GetMessageBoxStandardWindow("Confirm Deletion", $"Delete atlas '{atlas.Id}'?", MessageBox.Avalonia.Enums.ButtonEnum.OkCancel,
                    MessageBox.Avalonia.Enums.Icon.Warning);
            var buttonResult = await alertBox.Show();

            if (buttonResult == MessageBox.Avalonia.Enums.ButtonResult.Ok)
            {
                Atlas.Remove(atlas);
                Unsaved = true;
            }
        }
    }
}