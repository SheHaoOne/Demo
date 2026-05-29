using System.Collections.ObjectModel;
using FlowDesk.Abstractions;
using FlowDesk.App.ViewModels.Shared;
using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels.Editor;

/// <summary>
/// 工作流步骤 ViewModel。
/// </summary>
public sealed class WorkflowStepViewModel : ObservableObject
{
    private string _displayName;

    public WorkflowStepViewModel(WorkflowStepDefinition model)
    {
        Model = model;
        _displayName = model.DisplayName;
        Settings = new ObservableCollection<KeyValueViewModel>(
            model.Settings.Select(pair => new KeyValueViewModel(pair.Key, pair.Value)));
    }

    public WorkflowStepDefinition Model { get; }
    public ObservableCollection<KeyValueViewModel> Settings { get; }

    public string DisplayName
    {
        get => _displayName;
        set
        {
            if (SetProperty(ref _displayName, value))
                Model.DisplayName = value;
        }
    }

    public string PluginId => Model.PluginId;

    public void SyncSettingsToModel()
    {
        Model.Settings = Settings.ToDictionary(
            s => s.Key,
            s => s.Value,
            StringComparer.OrdinalIgnoreCase);
    }
}
