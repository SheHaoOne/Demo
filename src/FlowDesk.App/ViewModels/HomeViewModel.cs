using System.Collections.ObjectModel;
using System.Diagnostics;
using FlowDesk.Abstractions;
using FlowDesk.Core;
using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels;

/// <summary>
/// 首页 ViewModel：运行序列，树状表格显示步骤状态，日志输出。
/// </summary>
public sealed class HomeViewModel : ObservableObject
{
    private readonly PluginCatalog _catalog;
    private WorkflowSequence? _sequence;
    private bool _isRunning;
    private string _status = "就绪";
    private string _sequenceName = "（未加载序列）";

    public HomeViewModel(PluginCatalog catalog)
    {
        _catalog = catalog;
        StepNodes = [];
        LogEntries = [];

        RunCommand = new RelayCommand(async _ => await RunAsync(), _ => _sequence is not null && !IsRunning);
        StopCommand = new RelayCommand(_ => RequestStop(), _ => IsRunning);
    }

    public ObservableCollection<StepNodeViewModel> StepNodes { get; }

    public ObservableCollection<string> LogEntries { get; }

    public RelayCommand RunCommand { get; }

    public RelayCommand StopCommand { get; }

    private CancellationTokenSource? _cts;

    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (SetProperty(ref _isRunning, value))
            {
                RunCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    public string SequenceName
    {
        get => _sequenceName;
        private set => SetProperty(ref _sequenceName, value);
    }

    /// <summary>
    /// 从序列编辑器或文件加载序列后调用，准备运行视图。
    /// </summary>
    public void LoadSequence(WorkflowSequence sequence)
    {
        _sequence = sequence;
        SequenceName = sequence.Name;
        StepNodes.Clear();
        foreach (var step in sequence.Steps)
        {
            StepNodes.Add(new StepNodeViewModel(step.DisplayName, step.PluginId));
        }
        Status = "序列已加载，可以运行。";
        RunCommand.RaiseCanExecuteChanged();
    }

    private async Task RunAsync()
    {
        if (_sequence is null) return;

        IsRunning = true;
        _cts = new CancellationTokenSource();
        LogEntries.Clear();
        Status = "正在运行...";

        foreach (var node in StepNodes)
        {
            node.Status = "等待中";
            node.Duration = "";
            node.Message = "";
        }

        try
        {
            var runner = new WorkflowRunner(_catalog);
            var stepIndex = 0;
            var sw = new Stopwatch();

            runner.StepCompleted += (_, entry) =>
            {
                if (stepIndex < StepNodes.Count)
                {
                    var node = StepNodes[stepIndex];
                    node.Status = entry.Succeeded ? "成功" : "失败";
                    node.Message = entry.Message;
                    node.Duration = sw.ElapsedMilliseconds + " ms";
                }
                var icon = entry.Succeeded ? "OK" : "FAIL";
                LogEntries.Add($"{entry.Timestamp:HH:mm:ss} [{icon}] {entry.StepDisplayName}: {entry.Message}");
                stepIndex++;
            };

            for (var i = 0; i < StepNodes.Count; i++)
            {
                StepNodes[i].Status = i == 0 ? "运行中" : "等待中";
            }

            sw.Start();
            var entries = await runner.RunAsync(_sequence, _cts.Token);
            sw.Stop();

            Status = entries.All(e => e.Succeeded)
                ? $"运行完成，耗时 {sw.ElapsedMilliseconds} ms"
                : "运行中止（存在失败步骤）";
        }
        catch (OperationCanceledException)
        {
            Status = "运行已取消";
            LogEntries.Add("--- 用户取消了运行 ---");
        }
        finally
        {
            IsRunning = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void RequestStop()
    {
        _cts?.Cancel();
    }
}

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
