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
        public Interaction<MainWindowViewModel, string?> ShowOpenAtlasDialog { get; }

        public ReactiveCommand<Unit, Unit> DoNewDocument { get; }
        public ReactiveCommand<Unit, Unit> DoOpenDocument { get; }
        public ReactiveCommand<Unit, Unit> DoSaveDocument { get; }
        public ReactiveCommand<Unit, Unit> DoSaveDocumentAs { get; }
        public ReactiveCommand<Unit, Unit> DoNewAtlas { get; }
        public ReactiveCommand<EditorSpriteAtlas, Unit> DoDeleteAtlas { get; }
        public ReactiveCommand<Unit, Unit> DoGridSlice { get; }
        public ReactiveCommand<Unit, Unit> DoImportAtlas { get; }

        private EditorDocument _activeDocument;
        private EditorSpriteAtlas? _activeAtlas;
        private string? _activeDocumentPath;
        private bool _unsaved;
        private int _gridSliceRows;
        private int _gridSliceColumns;

        public EditorDocument ActiveDocument
        {
            get => _activeDocument;
            set
            {
                this.RaiseAndSetIfChanged(ref _activeDocument, value);
                this.RaisePropertyChanged("WindowTitle");
            }
        }
        public EditorSpriteAtlas? ActiveAtlas
        {
            get => _activeAtlas;
            set
            {
                this.RaiseAndSetIfChanged(ref _activeAtlas, value);
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
        public int GridSliceRows
        {
            get => _gridSliceRows;
            set
            {
                this.RaiseAndSetIfChanged(ref _gridSliceRows, value);
            }
        }
        public int GridSliceColumns
        {
            get => _gridSliceColumns;
            set
            {
                this.RaiseAndSetIfChanged(ref _gridSliceColumns, value);
            }
        }
        public string WindowTitle => $"GaSpTK Editor - {_activeDocumentPath ?? "Untitled Document"} {(_unsaved ? "*" : "")}";
        public string RootPath => System.IO.Path.GetDirectoryName(ActiveDocumentPath)!;

        public MainWindowViewModel()
        {
            Activator = new ViewModelActivator();
            
            ShowOpenDialog = new Interaction<MainWindowViewModel, string?>();
            ShowSaveDialog = new Interaction<MainWindowViewModel, string?>();
            ShowOpenImageDialog = new Interaction<MainWindowViewModel, string?>();
            ShowOpenAtlasDialog = new Interaction<MainWindowViewModel, string?>();

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
            DoDeleteAtlas = ReactiveCommand.CreateFromTask((EditorSpriteAtlas atlas) =>
            {
                return DeleteAtlas(atlas);
            });
            DoGridSlice = ReactiveCommand.CreateFromTask(() =>
            {
                return GridSlice();
            });
            DoImportAtlas = ReactiveCommand.CreateFromTask(() =>
            {
                return ImportAtlas();
            });

            _activeDocumentPath = null;
            _activeAtlas = null;
            _activeDocument = new EditorDocument(new File());
            _unsaved = true;

            _gridSliceRows = 1;
            _gridSliceColumns = 1;

            _activeDocument.OnModified += () =>
            {
                Unsaved = true;
            };
        }

        public void NewDocument()
        {
            ActiveDocument = new EditorDocument(new File());
            ActiveAtlas = null;
            ActiveDocumentPath = null;
            Unsaved = true;

            _activeDocument.OnModified += () =>
            {
                Unsaved = true;
            };
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
                ActiveDocument = new EditorDocument(file);
                ActiveAtlas = null;
                Unsaved = false;

                foreach (var atlas in _activeDocument.Atlas)
                {
                    string fullPath = System.IO.Path.Combine(RootPath, atlas.Path);
                    atlas.CachedBmp = BitmapLoader.Convert(fullPath);
                }

                _activeDocument.OnModified += () =>
                {
                    Unsaved = true;
                };
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
                await System.IO.File.WriteAllTextAsync(path, ActiveDocument.ToJson());
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
                atlas.Id = System.IO.Path.GetFileNameWithoutExtension(imgPath);
                atlas.Path = System.IO.Path.GetRelativePath(RootPath, imgPath);

                var editorAtlas = new EditorSpriteAtlas(atlas, ActiveDocument);
                editorAtlas.CachedBmp = BitmapLoader.Convert(imgPath);

                ActiveDocument.Atlas.Add(editorAtlas);
                Unsaved = true;
            }
        }

        public async Task DeleteAtlas(EditorSpriteAtlas atlas)
        {
            var alertBox = MessageBoxManager.GetMessageBoxStandardWindow("Confirm Deletion", $"Delete atlas '{atlas.Id}'?", MessageBox.Avalonia.Enums.ButtonEnum.OkCancel,
                    MessageBox.Avalonia.Enums.Icon.Warning);
            var buttonResult = await alertBox.Show();

            if (buttonResult == MessageBox.Avalonia.Enums.ButtonResult.Ok)
            {
                atlas.CachedBmp?.Dispose();
                ActiveDocument.Atlas.Remove(atlas);
                Unsaved = true;
            }
        }

        public void NewSprite()
        {
            ActiveAtlas!.Sprites.Add(new EditorSprite(new Sprite()
            {
                Id = "New Sprite"
            }, ActiveAtlas));
            Unsaved = true;
        }

        public void DeleteSprite(EditorSprite sprite)
        {
            ActiveAtlas!.Sprites.Remove(sprite);
            Unsaved = true;
        }

        public async Task GridSlice()
        {
            var alertBox = MessageBoxManager.GetMessageBoxStandardWindow("Confirm Operation", $"Create new sprites from grid slices? This will erase any existing sprites!", MessageBox.Avalonia.Enums.ButtonEnum.OkCancel,
                MessageBox.Avalonia.Enums.Icon.Warning);
            var buttonResult = await alertBox.Show();

            if (buttonResult == MessageBox.Avalonia.Enums.ButtonResult.Ok)
            {
                ActiveAtlas!.Sprites.Clear();
                var cellWidth = (int)(ActiveAtlas!.CachedBmp!.Size.Width / GridSliceColumns);
                var cellHeight = (int)(ActiveAtlas!.CachedBmp!.Size.Height / GridSliceRows);

                for (int j = 0; j < GridSliceRows; j++)
                {
                    for (int i = 0; i < GridSliceColumns; i++)
                    {
                        var x = i * cellWidth;
                        var y = i * cellHeight;

                        ActiveAtlas!.Sprites.Add(new EditorSprite(new Sprite()
                        {
                            Id = $"sprite_{i}_{j}",
                            X = i * cellWidth,
                            Y = j * cellHeight,
                            Width = cellWidth,
                            Height = cellHeight
                        }, ActiveAtlas));
                    }
                }

                Unsaved = true;
            }
        }

        public async Task ImportAtlas()
        {
            // NOTE: in order to import a new atlas, current document must be saved
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

            var atlasPath = await ShowOpenAtlasDialog.Handle(this);

            if (atlasPath != null)
            {
                // try to load TexturePacker json hash sprite sheet
                try
                {
                    var sheetJson = System.IO.File.ReadAllText(atlasPath);
                    var texturePackerData = JsonConvert.DeserializeObject<TexturePackerFile>(sheetJson)!;

                    var targetPath = System.IO.Path.GetDirectoryName(atlasPath)!;
                    var imgPath = System.IO.Path.Combine(targetPath, texturePackerData.meta.image);

                    var spriteAtlas = new SpriteAtlas();
                    spriteAtlas.Id = System.IO.Path.GetFileNameWithoutExtension(atlasPath);
                    spriteAtlas.Path = System.IO.Path.GetRelativePath(RootPath, imgPath);
                    
                    foreach (var kvp in texturePackerData.frames)
                    {
                        if (kvp.Value.rotated)
                        {
                            throw new System.NotImplementedException("Sprite rotation feature not supported");
                        }

                        if (kvp.Value.trimmed)
                        {
                            throw new System.NotImplementedException("Sprite trim feature not supported");
                        }

                        var sprite = new Sprite();
                        sprite.Id = kvp.Key;
                        sprite.X = kvp.Value.frame.x;
                        sprite.Y = kvp.Value.frame.y;
                        sprite.Width = kvp.Value.frame.w;
                        sprite.Height = kvp.Value.frame.h;

                        spriteAtlas.Sprites.Add(sprite);
                    }

                    var editorAtlas = new EditorSpriteAtlas(spriteAtlas, ActiveDocument);
                    editorAtlas.CachedBmp = BitmapLoader.Convert(imgPath);

                    ActiveDocument.Atlas.Add(editorAtlas);
                    Unsaved = true;
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
                catch (System.NotImplementedException e)
                {
                    var alertBox = MessageBoxManager.GetMessageBoxStandardWindow("Error", e.Message);
                    await alertBox.Show();
                }
            }
        }
    }
}