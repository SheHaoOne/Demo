namespace FlowDesk.Abstractions;

/// <summary>
/// 工作流执行引擎契约。
/// </summary>
public interface IWorkflowRunner
{
    event EventHandler<WorkflowExecutionLogEntry>? StepCompleted;
    Task<IReadOnlyList<WorkflowExecutionLogEntry>> RunAsync(WorkflowSequence sequence, CancellationToken cancellationToken = default);
}
