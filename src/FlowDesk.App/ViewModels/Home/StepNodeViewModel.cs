using System.Collections.ObjectModel;
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

    public StepNodeViewModel(string displayName, string pluginId)
    {
        DisplayName = displayName;
        PluginId = pluginId;
        Children = [];
    }

    public string DisplayName { get; }
    public string PluginId { get; }
    public ObservableCollection<StepNodeViewModel> Children { get; }

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
}
