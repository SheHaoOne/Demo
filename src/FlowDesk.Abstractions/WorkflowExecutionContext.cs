namespace FlowDesk.Abstractions;

public sealed class WorkflowExecutionContext
{
    public WorkflowExecutionContext(
        WorkflowSequence sequence,
        WorkflowStepDefinition step,
        IDictionary<string, object?> variables)
    {
        Sequence = sequence;
        Step = step;
        Variables = variables;
    }

    public WorkflowSequence Sequence { get; }

    public WorkflowStepDefinition Step { get; }

    public IDictionary<string, object?> Variables { get; }
}
