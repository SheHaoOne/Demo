using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FlowDesk.App.ViewModels.Editor;

namespace FlowDesk.App.Views.Editor;

public partial class SequenceEditorPage : UserControl
{
    public SequenceEditorPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 点击插件卡片时选中该插件。
    /// </summary>
    private void PluginCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PluginCardViewModel card }
            && DataContext is SequenceEditorViewModel vm)
        {
            vm.SelectedPlugin = card;
        }
    }

    /// <summary>
    /// 鼠标拖动插件卡片时发起拖拽操作。
    /// </summary>
    private void PluginCard_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed
            && sender is FrameworkElement { DataContext: PluginCardViewModel card })
        {
            var data = new DataObject("PluginCard", card);
            DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Copy);
        }
    }

    /// <summary>
    /// 拖拽经过 TreeView 时判断是否接受放置。
    /// </summary>
    private void StepTree_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent("PluginCard")
            ? DragDropEffects.Copy
            : DragDropEffects.None;
        e.Handled = true;
    }

    /// <summary>
    /// 在 TreeView 上放置拖拽数据时添加插件到序列。
    /// </summary>
    private void StepTree_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData("PluginCard") is PluginCardViewModel card
            && DataContext is SequenceEditorViewModel vm)
        {
            vm.AddPluginByDrag(card);
            e.Handled = true;
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
