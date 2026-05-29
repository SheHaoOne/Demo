using System.Collections.ObjectModel;
using System.Diagnostics;
using FlowDesk.Abstractions;
using FlowDesk.App.Services;
using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels.Home;

/// <summary>
/// 首页 ViewModel：展示序列运行状态和日志（SRP：只管 UI 状态，执行委托给服务层）。
/// </summary>
public sealed class HomeViewModel : ObservableObject
{
    private readonly IWorkflowExecutionService _executionService;
    private WorkflowSequence? _sequence;
    private bool _isRunning;
    private string _status = "就绪";
    private string _sequenceName = "（未加载序列）";
    private CancellationTokenSource? _cts;

    public HomeViewModel(IWorkflowExecutionService executionService)
    {
        _executionService = executionService;
        StepNodes = [];
        LogEntries = [];
        RunCommand = new RelayCommand(async _ => await RunAsync(), _ => _sequence is not null && !IsRunning);
        StopCommand = new RelayCommand(_ => RequestStop(), _ => IsRunning);
    }

    public ObservableCollection<StepNodeViewModel> StepNodes { get; }
    public ObservableCollection<string> LogEntries { get; }
    public RelayCommand RunCommand { get; }
    public RelayCommand StopCommand { get; }

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

    public void LoadSequence(WorkflowSequence sequence)
    {
        _sequence = sequence;
        SequenceName = sequence.Name;
        StepNodes.Clear();
        foreach (var step in sequence.Steps)
            StepNodes.Add(new StepNodeViewModel(step.DisplayName, step.PluginId));
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
            var stepIndex = 0;
            var sw = Stopwatch.StartNew();

            void OnStepCompleted(object? sender, WorkflowExecutionLogEntry entry)
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
            }

            _executionService.StepCompleted += OnStepCompleted;
            try
            {
                var entries = await _executionService.RunAsync(_sequence, _cts.Token);
                sw.Stop();
                Status = entries.All(e => e.Succeeded)
                    ? $"运行完成，耗时 {sw.ElapsedMilliseconds} ms"
                    : "运行中止（存在失败步骤）";
            }
            finally
            {
                _executionService.StepCompleted -= OnStepCompleted;
            }
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

    private void RequestStop() => _cts?.Cancel();
}
