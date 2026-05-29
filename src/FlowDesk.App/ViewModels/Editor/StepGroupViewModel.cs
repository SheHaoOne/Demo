using System.Collections.ObjectModel;
using FlowDesk.Abstractions;
using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels.Editor;

/// <summary>
/// 步骤分组 ViewModel。
/// </summary>
public sealed class StepGroupViewModel : ObservableObject
{
    private string _name;

    public StepGroupViewModel(WorkflowStepGroup model)
    {
        Model = model;
        _name = model.Name;
        Steps = new ObservableCollection<WorkflowStepViewModel>(
            model.Steps.Select(s => new WorkflowStepViewModel(s)));
    }

    public WorkflowStepGroup Model { get; }

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
                Model.Name = value;
        }
    }

    public ObservableCollection<WorkflowStepViewModel> Steps { get; }
}
