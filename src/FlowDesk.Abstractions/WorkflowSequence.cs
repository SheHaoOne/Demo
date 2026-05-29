namespace FlowDesk.Abstractions;

public sealed class WorkflowSequence
{
    public string Name { get; set; } = "Untitled workflow";
    public List<WorkflowStepGroup> Groups { get; set; } = [];
    public List<WorkflowStepDefinition> Steps { get; set; } = [];
}
