using System.Windows;
using FlowDesk.App.Composition;

namespace FlowDesk.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = AppCompositionRoot.Build();
    }
}
