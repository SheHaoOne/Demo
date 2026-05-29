using FlowDesk.Abstractions;

namespace FlowDesk.App.Services;

/// <summary>
/// 工作流执行服务契约。将执行编排逻辑从 ViewModel 中分离（SRP + DIP）。
/// </summary>
public interface IWorkflowExecutionService
{
    event EventHandler<WorkflowExecutionLogEntry>? StepCompleted;
    Task<IReadOnlyList<WorkflowExecutionLogEntry>> RunAsync(WorkflowSequence sequence, CancellationToken cancellationToken = default);
}
