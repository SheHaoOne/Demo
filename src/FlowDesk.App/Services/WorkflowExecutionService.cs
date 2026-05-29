using FlowDesk.Abstractions;

namespace FlowDesk.App.Services;

/// <summary>
/// 工作流执行服务实现，封装 IWorkflowRunner。
/// </summary>
public sealed class WorkflowExecutionService : IWorkflowExecutionService
{
    private readonly IWorkflowRunner _runner;

    public WorkflowExecutionService(IWorkflowRunner runner)
    {
        _runner = runner;
    }

    public event EventHandler<WorkflowExecutionLogEntry>? StepCompleted
    {
        add => _runner.StepCompleted += value;
        remove => _runner.StepCompleted -= value;
    }

    public Task<IReadOnlyList<WorkflowExecutionLogEntry>> RunAsync(
        WorkflowSequence sequence, CancellationToken cancellationToken = default)
    {
        return _runner.RunAsync(sequence, cancellationToken);
    }
}
