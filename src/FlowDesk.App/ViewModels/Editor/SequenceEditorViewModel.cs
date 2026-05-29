using System.Collections.ObjectModel;
using FlowDesk.Abstractions;
using FlowDesk.App.Services;
using FlowDesk.App.ViewModels.Shared;
using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels.Editor;

/// <summary>
/// 序列编辑器 ViewModel：插件分类卡片 + 序列树状列表 + 文件操作。
/// </summary>
public sealed class SequenceEditorViewModel : ObservableObject
{
    private const string FileFilter = "FlowDesk 工作流 (*.flow.json)|*.flow.json|JSON 文件 (*.json)|*.json";

    private readonly IWorkflowSerializer _serializer;
    private readonly IFileDialogService _fileDialog;
    private WorkflowSequence _sequence = new();
    private string _sequenceName = "未命名工作流";
    private string? _currentFilePath;
    private PluginCardViewModel? _selectedPlugin;
    private WorkflowStepViewModel? _selectedStep;

    /// <summary>
    /// 序列准备就绪事件（供外部监听以传递给运行页面）。
    /// </summary>
    public event EventHandler<WorkflowSequence>? SequenceReady;

    public SequenceEditorViewModel(IPluginCatalog catalog, IWorkflowSerializer serializer, IFileDialogService fileDialog)
    {
        _serializer = serializer;
        _fileDialog = fileDialog;

        var groups = catalog.Plugins
            .GroupBy(p => p.Descriptor.Category, StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

        PluginCategories = new ObservableCollection<PluginCategoryViewModel>(
            groups.Select(g => new PluginCategoryViewModel(
                g.Key,
                new ObservableCollection<PluginCardViewModel>(
                    g.Select(p => new PluginCardViewModel(p))))));

        Steps = [];
        StepGroups = [];

        NewCommand = new RelayCommand(_ => NewSequence());
        OpenCommand = new RelayCommand(async _ => await OpenAsync());
        SaveCommand = new RelayCommand(async _ => await SaveAsync());
        SaveAsCommand = new RelayCommand(async _ => await SaveAsAsync());
        AddPluginCommand = new RelayCommand(_ => AddSelectedPlugin(), _ => SelectedPlugin is not null);
        RemoveStepCommand = new RelayCommand(_ => RemoveSelectedStep(), _ => SelectedStep is not null);
        MoveStepUpCommand = new RelayCommand(_ => MoveSelectedStep(-1), _ => CanMoveStep(-1));
        MoveStepDownCommand = new RelayCommand(_ => MoveSelectedStep(1), _ => CanMoveStep(1));
        AddGroupCommand = new RelayCommand(_ => AddGroup());
        SendToHomeCommand = new RelayCommand(_ => SendToHome());
    }

    public ObservableCollection<PluginCategoryViewModel> PluginCategories { get; }
    public ObservableCollection<WorkflowStepViewModel> Steps { get; }
    public ObservableCollection<StepGroupViewModel> StepGroups { get; }

    public RelayCommand NewCommand { get; }
    public RelayCommand OpenCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand SaveAsCommand { get; }
    public RelayCommand AddPluginCommand { get; }
    public RelayCommand RemoveStepCommand { get; }
    public RelayCommand MoveStepUpCommand { get; }
    public RelayCommand MoveStepDownCommand { get; }
    public RelayCommand AddGroupCommand { get; }
    public RelayCommand SendToHomeCommand { get; }

    public string SequenceName
    {
        get => _sequenceName;
        set
        {
            if (SetProperty(ref _sequenceName, value))
                _sequence.Name = value;
        }
    }

    public PluginCardViewModel? SelectedPlugin
    {
        get => _selectedPlugin;
        set
        {
            if (SetProperty(ref _selectedPlugin, value))
                AddPluginCommand.RaiseCanExecuteChanged();
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
                RefreshStepCommands();
            }
        }
    }

    public ObservableCollection<KeyValueViewModel>? SelectedStepSettings => SelectedStep?.Settings;

    private void NewSequence()
    {
        _sequence = new WorkflowSequence();
        _currentFilePath = null;
        SequenceName = "未命名工作流";
        Steps.Clear();
        StepGroups.Clear();
        SelectedStep = null;
        RefreshStepCommands();
    }

    private async Task OpenAsync()
    {
        var path = _fileDialog.ShowOpenDialog(FileFilter, "打开工作流");
        if (path is null) return;

        var sequence = await _serializer.LoadAsync(path);
        _sequence = sequence;
        _currentFilePath = path;
        SequenceName = sequence.Name;

        Steps.Clear();
        foreach (var step in sequence.Steps)
            Steps.Add(new WorkflowStepViewModel(step));

        StepGroups.Clear();
        foreach (var group in sequence.Groups)
            StepGroups.Add(new StepGroupViewModel(group));

        SelectedStep = null;
        RefreshStepCommands();
    }

    private async Task SaveAsync()
    {
        if (_currentFilePath is null)
        {
            await SaveAsAsync();
            return;
        }
        SyncSequenceFromEditor();
        await _serializer.SaveAsync(_sequence, _currentFilePath);
    }

    private async Task SaveAsAsync()
    {
        var path = _fileDialog.ShowSaveDialog(FileFilter, $"{SequenceName}.flow.json", "另存为");
        if (path is null) return;

        _currentFilePath = path;
        SyncSequenceFromEditor();
        await _serializer.SaveAsync(_sequence, path);
    }

    private void AddGroup()
    {
        var group = new WorkflowStepGroup { Name = $"分组 {StepGroups.Count + 1}" };
        _sequence.Groups.Add(group);
        StepGroups.Add(new StepGroupViewModel(group));
    }

    private void AddSelectedPlugin()
    {
        if (SelectedPlugin is null) return;
        var plugin = SelectedPlugin.Plugin;
        var step = new WorkflowStepDefinition
        {
            PluginId = plugin.Descriptor.Id,
            DisplayName = plugin.Descriptor.DisplayName,
            Settings = new Dictionary<string, string>(plugin.DefaultSettings, StringComparer.OrdinalIgnoreCase)
        };
        var vm = new WorkflowStepViewModel(step);
        Steps.Add(vm);
        SelectedStep = vm;
        RefreshStepCommands();
    }

    private void RemoveSelectedStep()
    {
        if (SelectedStep is null) return;
        var idx = Steps.IndexOf(SelectedStep);
        Steps.Remove(SelectedStep);
        SelectedStep = Steps.Count == 0 ? null : Steps[Math.Min(idx, Steps.Count - 1)];
        RefreshStepCommands();
    }

    private void MoveSelectedStep(int offset)
    {
        if (SelectedStep is null) return;
        var oldIdx = Steps.IndexOf(SelectedStep);
        var newIdx = oldIdx + offset;
        if (newIdx < 0 || newIdx >= Steps.Count) return;
        Steps.Move(oldIdx, newIdx);
        RefreshStepCommands();
    }

    private bool CanMoveStep(int offset)
    {
        if (SelectedStep is null) return false;
        var newIdx = Steps.IndexOf(SelectedStep) + offset;
        return newIdx >= 0 && newIdx < Steps.Count;
    }

    private void SendToHome()
    {
        SyncSequenceFromEditor();
        SequenceReady?.Invoke(this, _sequence);
    }

    private void SyncSequenceFromEditor()
    {
        _sequence.Name = SequenceName;
        _sequence.Steps = Steps.Select(s =>
        {
            s.SyncSettingsToModel();
            return s.Model;
        }).ToList();
    }

    private void RefreshStepCommands()
    {
        RemoveStepCommand.RaiseCanExecuteChanged();
        MoveStepUpCommand.RaiseCanExecuteChanged();
        MoveStepDownCommand.RaiseCanExecuteChanged();
    }
}
