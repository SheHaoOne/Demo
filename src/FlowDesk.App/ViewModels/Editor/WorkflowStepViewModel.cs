using System.Collections.ObjectModel;
using FlowDesk.Abstractions;
using FlowDesk.App.ViewModels.Editor.PropertyEditors;
using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels.Editor;

/// <summary>
/// 工作流步骤 ViewModel，使用属性编辑器替代硬编码键值对。
/// </summary>
public sealed class WorkflowStepViewModel : ObservableObject
{
    private string _displayName;

    public WorkflowStepViewModel(WorkflowStepDefinition model, IReadOnlyList<StepPropertyDescriptor>? descriptors = null)
    {
        Model = model;
        _displayName = model.DisplayName;

        if (descriptors is not null && descriptors.Count > 0)
        {
            PropertyEditors = new ObservableCollection<PropertyEditorViewModel>(
                descriptors.Select(d =>
                {
                    var currentValue = model.Settings.TryGetValue(d.Name, out var v)
                        ? v
                        : d.DefaultValue?.ToString() ?? "";
                    return PropertyEditorViewModel.Create(d, currentValue);
                }));
        }
        else
        {
            // 向后兼容：无描述符时从 Settings 字典生成字符串编辑器
            PropertyEditors = new ObservableCollection<PropertyEditorViewModel>(
                model.Settings.Select(kvp =>
                    PropertyEditorViewModel.Create(
                        new StepPropertyDescriptor { Name = kvp.Key, DisplayName = kvp.Key },
                        kvp.Value)));
        }
    }

    public WorkflowStepDefinition Model { get; }
    public ObservableCollection<PropertyEditorViewModel> PropertyEditors { get; }

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

    /// <summary>
    /// 将属性编辑器的值同步回 Model.Settings 字典。
    /// </summary>
    public void SyncSettingsToModel()
    {
        Model.Settings = PropertyEditors.ToDictionary(
            e => e.Name,
            e => e.StringValue,
            StringComparer.OrdinalIgnoreCase);
    }
}
