using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Avalonia.Interactivity;
using Avalonia.Input;
using ReactiveUI;
using MessageBox.Avalonia;
using GaSpTK.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GaSpTK.Editor
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private Image _atlasPreview;

        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowOpenDialog.RegisterHandler(DoShowOpenDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowSaveDialog.RegisterHandler(DoShowSaveDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowOpenImageDialog.RegisterHandler(DoShowOpenImageDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowOpenAtlasDialog.RegisterHandler(DoShowOpenAtlasDialogAsync)));
            this.Closing += (sender, args) =>
            {
                if (ViewModel!.Unsaved)
                {
                    args.Cancel = true;
                    Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        var alertBox = MessageBoxManager.GetMessageBoxStandardWindow("Quit", $"Document has unsaved changes. Are you sure you want to quit?", MessageBox.Avalonia.Enums.ButtonEnum.OkCancel,
                            MessageBox.Avalonia.Enums.Icon.Warning);

                        var buttonResult = await alertBox.Show();

                        if (buttonResult == MessageBox.Avalonia.Enums.ButtonResult.Ok)
                        {
                            // silly hack but avoids re-triggering this logic
                            ViewModel.Unsaved = false;
                            this.Close();
                        }
                    });
                }
            };

            _atlasPreview = this.FindControl<Image>("AtlasImage");
            var atlasList = this.FindControl<ListBox>("AtlasList");
            var atlasInspector = this.FindControl<Control>("AtlasInspector");
            var atlasInspectorFallback = this.FindControl<Control>("AtlasInspectorFallback");

            atlasList.SelectionChanged += (sender, e) =>
            {
                if (atlasList.SelectedIndex == -1)
                {
                    _atlasPreview.Source = null;
                    atlasInspector.IsVisible = false;
                    atlasInspectorFallback.IsVisible = true;

                    ViewModel!.ActiveAtlas = null;
                }
                else
                {
                    var atlas = this.ViewModel!.ActiveDocument.Atlas[atlasList.SelectedIndex];
                    var imgPath = System.IO.Path.Combine(this.ViewModel!.RootPath, atlas.Path);
                    _atlasPreview.Source = atlas.CachedBmp;

                    atlasInspector.IsVisible = true;
                    atlasInspectorFallback.IsVisible = false;

                    ViewModel!.ActiveAtlas = atlas;
                }
            };
        }

        private async Task DoShowOpenDialogAsync(InteractionContext<MainWindowViewModel, string?> interactionContext)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Title = "Open File";
            openDialog.Directory = System.Environment.CurrentDirectory;
            List<FileDialogFilter> filters = new List<FileDialogFilter>();
            FileDialogFilter filter = new FileDialogFilter();
            List<string> extension = new List<string>();
            extension.Add("json");
            filter.Extensions = extension;
            filter.Name = "GaSp Sprite Files";
            filters.Add(filter);
            openDialog.Filters = filters;

            var files = await openDialog.ShowAsync(this);

            if (files != null && files.Length > 0)
            {
                interactionContext.SetOutput(files[0]);
                return;
            }

            interactionContext.SetOutput(null);
            return;
        }

        private async Task DoShowOpenAtlasDialogAsync(InteractionContext<MainWindowViewModel, string?> interactionContext)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Title = "Open TexturePacker Sheet";
            openDialog.Directory = interactionContext.Input.ActiveDocumentPath == null ? System.Environment.CurrentDirectory :
                System.IO.Path.GetDirectoryName(interactionContext.Input.ActiveDocumentPath);
                
            List<FileDialogFilter> filters = new List<FileDialogFilter>();
            FileDialogFilter filter = new FileDialogFilter();
            List<string> extension = new List<string>();
            extension.Add("json");
            filter.Extensions = extension;
            filter.Name = "TexturePacker sheet (json hash)";
            filters.Add(filter);
            openDialog.Filters = filters;

            var files = await openDialog.ShowAsync(this);

            if (files != null && files.Length > 0)
            {
                interactionContext.SetOutput(files[0]);
                return;
            }

            interactionContext.SetOutput(null);
            return;
        }

        private async Task DoShowOpenImageDialogAsync(InteractionContext<MainWindowViewModel, string?> interactionContext)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Title = "Open Image";
            openDialog.Directory = interactionContext.Input.ActiveDocumentPath == null ? System.Environment.CurrentDirectory :
                System.IO.Path.GetDirectoryName(interactionContext.Input.ActiveDocumentPath);

            List<FileDialogFilter> filters = new List<FileDialogFilter>();
            FileDialogFilter filter = new FileDialogFilter();
            List<string> extension = new List<string>();
            extension.Add("png");
            extension.Add("jpg");
            extension.Add("jpeg");
            extension.Add("qoi");
            filter.Extensions = extension;
            filter.Name = "Image Files";
            filters.Add(filter);
            openDialog.Filters = filters;

            var files = await openDialog.ShowAsync(this);

            if (files != null && files.Length > 0)
            {
                interactionContext.SetOutput(files[0]);
                return;
            }

            interactionContext.SetOutput(null);
            return;
        }

        private async Task DoShowSaveDialogAsync(InteractionContext<MainWindowViewModel, string?> interactionContext)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save File";
            saveDialog.InitialFileName = interactionContext.Input.ActiveDocumentPath ?? "New Sprite File.json";
            saveDialog.Directory = System.Environment.CurrentDirectory;
            List<FileDialogFilter> filters = new List<FileDialogFilter>();
            FileDialogFilter filter = new FileDialogFilter();
            List<string> extension = new List<string>();
            extension.Add("json");
            filter.Extensions = extension;
            filter.Name = "GaSp Sprite Files";
            filters.Add(filter);
            saveDialog.Filters = filters;
            saveDialog.DefaultExtension = "json";

            var file = await saveDialog.ShowAsync(this);
            interactionContext.SetOutput(file);
        }
    }
}