using System.Collections.ObjectModel;
using System.Windows.Input;
using FlowDesk.Abstractions;
using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels.Home;

/// <summary>
/// 步骤执行状态树节点。
/// </summary>
public sealed class StepNodeViewModel : ObservableObject
{
    private string _status = "等待中";
    private string _duration = "";
    private string _message = "";
    private bool _hasChartData;
    private TdmsFileData? _chartData;

    public StepNodeViewModel(string displayName, string pluginId)
    {
        DisplayName = displayName;
        PluginId = pluginId;
        Children = [];
        ViewChartCommand = new RelayCommand(_ => ViewChart(), _ => HasChartData);
    }

    public string DisplayName { get; }
    public string PluginId { get; }
    public ObservableCollection<StepNodeViewModel> Children { get; }
    public RelayCommand ViewChartCommand { get; }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public string Duration
    {
        get => _duration;
        set => SetProperty(ref _duration, value);
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    /// <summary>
    /// 是否有可查看的图表数据。
    /// </summary>
    public bool HasChartData
    {
        get => _hasChartData;
        private set
        {
            if (SetProperty(ref _hasChartData, value))
                ViewChartCommand.RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// 设置 TDMS 图表数据（执行完成后由 HomeViewModel 调用）。
    /// </summary>
    public void SetChartData(TdmsFileData data)
    {
        _chartData = data;
        HasChartData = true;
    }

    /// <summary>
    /// 查看图表事件（由 View 层订阅并打开窗口）。
    /// </summary>
    public event EventHandler<TdmsFileData>? ChartRequested;

    private void ViewChart()
    {
        if (_chartData is not null)
            ChartRequested?.Invoke(this, _chartData);
    }
}
