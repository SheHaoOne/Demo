using FlowDesk.Abstractions;

namespace FlowDesk.Core;

public sealed class WorkflowRunner
{
    private readonly PluginCatalog _catalog;

    public WorkflowRunner(PluginCatalog catalog)
    {
        _catalog = catalog;
    }

    public event EventHandler<WorkflowExecutionLogEntry>? StepCompleted;

    public async Task<IReadOnlyList<WorkflowExecutionLogEntry>> RunAsync(
        WorkflowSequence sequence,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        var variables = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var entries = new List<WorkflowExecutionLogEntry>();

        foreach (var step in sequence.Steps)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await ExecuteStepAsync(sequence, step, variables, cancellationToken);
            var entry = new WorkflowExecutionLogEntry(
                DateTimeOffset.UtcNow,
                step.InstanceId,
                step.DisplayName,
                result.Succeeded,
                result.Message);

            entries.Add(entry);
            StepCompleted?.Invoke(this, entry);

            if (!result.Succeeded)
            {
                break;
            }
        }

        return entries;
    }

    private async ValueTask<StepExecutionResult> ExecuteStepAsync(
        WorkflowSequence sequence,
        WorkflowStepDefinition step,
        IDictionary<string, object?> variables,
        CancellationToken cancellationToken)
    {
        if (!_catalog.TryGet(step.PluginId, out var plugin))
        {
            return StepExecutionResult.Failure($"Plugin '{step.PluginId}' is not registered.");
        }

        try
        {
            var context = new WorkflowExecutionContext(sequence, step, variables);
            return await plugin.CreateExecutor().ExecuteAsync(context, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return StepExecutionResult.Failure(exception.Message);
        }
    }
}
