using System.Collections.ObjectModel;
using FlowDesk.Abstractions;
using FlowDesk.App.Services;
using FlowDesk.Core;

namespace FlowDesk.App.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly PluginCatalog _catalog;
    private readonly WorkflowFileService _fileService;
    private WorkflowSequence _sequence = new();
    private string _sequenceName = "Untitled workflow";
    private string _status = "Ready";
    private PluginViewModel? _selectedPlugin;
    private WorkflowStepViewModel? _selectedStep;
    private bool _isRunning;

    public MainWindowViewModel(PluginCatalog catalog, WorkflowFileService fileService)
    {
        _catalog = catalog;
        _fileService = fileService;

        Plugins = new ObservableCollection<PluginViewModel>(
            catalog.Plugins.Select(plugin => new PluginViewModel(plugin)));
        Steps = [];
        ExecutionLog = [];

        NewCommand = new RelayCommand(_ => NewWorkflow());
        OpenCommand = new RelayCommand(async _ => await OpenAsync(), _ => !IsRunning);
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => !IsRunning);
        AddPluginCommand = new RelayCommand(_ => AddSelectedPlugin(), _ => SelectedPlugin is not null && !IsRunning);
        RemoveStepCommand = new RelayCommand(_ => RemoveSelectedStep(), _ => SelectedStep is not null && !IsRunning);
        MoveStepUpCommand = new RelayCommand(_ => MoveSelectedStep(-1), _ => CanMoveSelectedStep(-1) && !IsRunning);
        MoveStepDownCommand = new RelayCommand(_ => MoveSelectedStep(1), _ => CanMoveSelectedStep(1) && !IsRunning);
        RunCommand = new RelayCommand(async _ => await RunAsync(), _ => Steps.Count > 0 && !IsRunning);
    }

    public ObservableCollection<PluginViewModel> Plugins { get; }

    public ObservableCollection<WorkflowStepViewModel> Steps { get; }

    public ObservableCollection<string> ExecutionLog { get; }

    public RelayCommand NewCommand { get; }

    public RelayCommand OpenCommand { get; }

    public RelayCommand SaveCommand { get; }

    public RelayCommand AddPluginCommand { get; }

    public RelayCommand RemoveStepCommand { get; }

    public RelayCommand MoveStepUpCommand { get; }

    public RelayCommand MoveStepDownCommand { get; }

    public RelayCommand RunCommand { get; }

    public string SequenceName
    {
        get => _sequenceName;
        set
        {
            if (SetProperty(ref _sequenceName, value))
            {
                _sequence.Name = value;
            }
        }
    }

    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    public PluginViewModel? SelectedPlugin
    {
        get => _selectedPlugin;
        set
        {
            if (SetProperty(ref _selectedPlugin, value))
            {
                RefreshCommands();
            }
        }
    }

    public WorkflowStepViewModel? SelectedStep
    {
        get => _selectedStep;
        set
        {
            if (SetProperty(ref _selectedStep, value))
            {
                OnPropertyChanged(nameof(SelectedStepSettings));
                RefreshCommands();
            }
        }
    }

    public ObservableCollection<SettingViewModel>? SelectedStepSettings => SelectedStep?.Settings;

    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (SetProperty(ref _isRunning, value))
            {
                RefreshCommands();
            }
        }
    }

    private void NewWorkflow()
    {
        _sequence = new WorkflowSequence();
        SequenceName = _sequence.Name;
        Steps.Clear();
        ExecutionLog.Clear();
        Status = "Created a new workflow.";
        RefreshCommands();
    }

    private async Task OpenAsync()
    {
        var sequence = await _fileService.OpenAsync();
        if (sequence is null)
        {
            return;
        }

        _sequence = sequence;
        SequenceName = sequence.Name;
        Steps.Clear();
        foreach (var step in sequence.Steps)
        {
            Steps.Add(new WorkflowStepViewModel(step));
        }

        ExecutionLog.Clear();
        Status = $"Loaded '{sequence.Name}'.";
        RefreshCommands();
    }

    private async Task SaveAsync()
    {
        SyncSequenceFromEditor();
        await _fileService.SaveAsync(_sequence);
        Status = $"Saved '{_sequence.Name}'.";
    }

    private void AddSelectedPlugin()
    {
        if (SelectedPlugin is null)
        {
            return;
        }

        var plugin = SelectedPlugin.Plugin;
        var step = new WorkflowStepDefinition
        {
            PluginId = plugin.Descriptor.Id,
            DisplayName = plugin.Descriptor.DisplayName,
            Settings = new Dictionary<string, string>(plugin.DefaultSettings, StringComparer.OrdinalIgnoreCase)
        };

        var viewModel = new WorkflowStepViewModel(step);
        Steps.Add(viewModel);
        SelectedStep = viewModel;
        Status = $"Added step '{step.DisplayName}'.";
        RefreshCommands();
    }

    private void RemoveSelectedStep()
    {
        if (SelectedStep is null)
        {
            return;
        }

        var index = Steps.IndexOf(SelectedStep);
        Steps.Remove(SelectedStep);
        SelectedStep = Steps.Count == 0 ? null : Steps[Math.Min(index, Steps.Count - 1)];
        Status = "Removed selected step.";
        RefreshCommands();
    }

    private void MoveSelectedStep(int offset)
    {
        if (SelectedStep is null)
        {
            return;
        }

        var oldIndex = Steps.IndexOf(SelectedStep);
        var newIndex = oldIndex + offset;
        if (newIndex < 0 || newIndex >= Steps.Count)
        {
            return;
        }

        Steps.Move(oldIndex, newIndex);
        Status = "Reordered workflow step.";
        RefreshCommands();
    }

    private async Task RunAsync()
    {
        SyncSequenceFromEditor();
        ExecutionLog.Clear();
        IsRunning = true;
        Status = "Running workflow...";

        try
        {
            var runner = new WorkflowRunner(_catalog);
            runner.StepCompleted += (_, entry) =>
            {
                var icon = entry.Succeeded ? "OK" : "FAIL";
                ExecutionLog.Add($"{entry.Timestamp:HH:mm:ss} [{icon}] {entry.StepDisplayName}: {entry.Message}");
            };

            var entries = await runner.RunAsync(_sequence);
            Status = entries.All(entry => entry.Succeeded)
                ? "Workflow completed successfully."
                : "Workflow stopped after a failed step.";
        }
        finally
        {
            IsRunning = false;
        }
    }

    private void SyncSequenceFromEditor()
    {
        _sequence.Name = SequenceName;
        _sequence.Steps = Steps
            .Select(step =>
            {
                step.SyncSettingsToModel();
                return step.Model;
            })
            .ToList();
    }

    private bool CanMoveSelectedStep(int offset)
    {
        if (SelectedStep is null)
        {
            return false;
        }

        var newIndex = Steps.IndexOf(SelectedStep) + offset;
        return newIndex >= 0 && newIndex < Steps.Count;
    }

    private void RefreshCommands()
    {
        NewCommand.RaiseCanExecuteChanged();
        OpenCommand.RaiseCanExecuteChanged();
        SaveCommand.RaiseCanExecuteChanged();
        AddPluginCommand.RaiseCanExecuteChanged();
        RemoveStepCommand.RaiseCanExecuteChanged();
        MoveStepUpCommand.RaiseCanExecuteChanged();
        MoveStepDownCommand.RaiseCanExecuteChanged();
        RunCommand.RaiseCanExecuteChanged();
    }
}
