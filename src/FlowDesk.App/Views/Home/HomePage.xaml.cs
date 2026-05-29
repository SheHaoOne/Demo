using System.Windows.Controls;
using FlowDesk.Abstractions;
using FlowDesk.App.ViewModels.Home;

namespace FlowDesk.App.Views.Home;

/// <summary>
/// 首页视图，负责订阅步骤节点的图表查看事件并打开波形窗口。
/// </summary>
public partial class HomePage : UserControl
{
    public HomePage()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is HomeViewModel oldVm)
        {
            foreach (var node in oldVm.StepNodes)
                node.ChartRequested -= OnChartRequested;
            oldVm.StepNodes.CollectionChanged -= StepNodes_CollectionChanged;
        }

        if (e.NewValue is HomeViewModel newVm)
        {
            foreach (var node in newVm.StepNodes)
                node.ChartRequested += OnChartRequested;
            newVm.StepNodes.CollectionChanged += StepNodes_CollectionChanged;
        }
    }

    private void StepNodes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (StepNodeViewModel node in e.NewItems)
                node.ChartRequested += OnChartRequested;
        }

        if (e.OldItems is not null)
        {
            foreach (StepNodeViewModel node in e.OldItems)
                node.ChartRequested -= OnChartRequested;
        }
    }

    private void OnChartRequested(object? sender, TdmsFileData data)
    {
        var window = new WaveformChartWindow(data);
        window.Owner = System.Windows.Window.GetWindow(this);
        window.Show();
    }
}
