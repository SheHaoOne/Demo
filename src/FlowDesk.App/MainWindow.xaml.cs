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

        var serializer = new JsonWorkflowSerializer();
        DataContext = new MainWindowViewModel(
            AppBootstrapper.CreatePluginCatalog(),
            new WorkflowFileService(serializer));
    }
}
