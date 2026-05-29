namespace FlowDesk.Abstractions;

public interface IWorkflowStepExecutor
{
    ValueTask<StepExecutionResult> ExecuteAsync(
        WorkflowExecutionContext context,
        CancellationToken cancellationToken);
}
