using System.Collections.ObjectModel;
using FlowDesk.Abstractions;

namespace FlowDesk.App.ViewModels;

public sealed class WorkflowStepViewModel : ObservableObject
{
    private string _displayName;

    public WorkflowStepViewModel(WorkflowStepDefinition model)
    {
        Model = model;
        _displayName = model.DisplayName;
        Settings = new ObservableCollection<SettingViewModel>(
            model.Settings.Select(pair => new SettingViewModel(pair.Key, pair.Value)));
    }

    public WorkflowStepDefinition Model { get; }

    public ObservableCollection<SettingViewModel> Settings { get; }

    public string DisplayName
    {
        get => _displayName;
        set
        {
            if (SetProperty(ref _displayName, value))
            {
                Model.DisplayName = value;
            }
        }
    }

    public string PluginId => Model.PluginId;

    public void SyncSettingsToModel()
    {
        Model.Settings = Settings.ToDictionary(
            setting => setting.Key,
            setting => setting.Value,
            StringComparer.OrdinalIgnoreCase);
    }
}

public sealed class SettingViewModel : ObservableObject
{
    private string _value;

    public SettingViewModel(string key, string value)
    {
        Key = key;
        _value = value;
    }

    public string Key { get; }

    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}
