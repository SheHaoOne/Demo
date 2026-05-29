using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FlowDesk.App.ViewModels;

namespace FlowDesk.App.Views;

public partial class SequenceEditorPage : UserControl
{
    public SequenceEditorPage()
    {
        InitializeComponent();
    }

    private void PluginCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PluginCardViewModel card }
            && DataContext is SequenceEditorViewModel vm)
        {
            vm.SelectedPlugin = card;
        }
    }

    private void StepTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (DataContext is SequenceEditorViewModel vm && e.NewValue is WorkflowStepViewModel step)
        {
            vm.SelectedStep = step;
        }
    }
}
