namespace FlowDesk.Abstractions;

public sealed class WorkflowSequence
{
    public string Name { get; set; } = "Untitled workflow";

    public List<WorkflowStepDefinition> Steps { get; set; } = [];
}
