using Avalonia.Controls;
using Avalonia.ReactiveUI;
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
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowOpenDialog.RegisterHandler(DoShowOpenDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowSaveDialog.RegisterHandler(DoShowSaveDialogAsync)));
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