using System.Windows;
using FlowDesk.App.Services;
using FlowDesk.App.ViewModels;
using FlowDesk.Core;

namespace FlowDesk.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var catalog = AppBootstrapper.CreatePluginCatalog();
        var serializer = new JsonWorkflowSerializer();

        var homePage = new HomeViewModel(catalog);
        var configPage = new ConfigViewModel();
        var editorPage = new SequenceEditorViewModel(catalog, serializer);

        editorPage.SequenceReady += (_, sequence) => homePage.LoadSequence(sequence);

        DataContext = new ShellViewModel(homePage, configPage, editorPage);
    }
}
