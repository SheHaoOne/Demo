using FlowDesk.Abstractions;

namespace FlowDesk.SamplePlugin;

public sealed class MessageStepPlugin : IWorkflowStepPlugin
{
    public PluginDescriptor Descriptor { get; } = new(
        "sample.message",
        "Message",
        "Samples",
        "Writes a message into the workflow execution log.",
        "1.0.0");

    public IReadOnlyDictionary<string, string> DefaultSettings { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["message"] = "Hello from the workflow."
        };

    public IWorkflowStepExecutor CreateExecutor() => new Executor();

    private sealed class Executor : IWorkflowStepExecutor
    {
        public ValueTask<StepExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var message = context.Step.Settings.TryGetValue("message", out var value)
                ? value
                : "Message step completed.";

            return ValueTask.FromResult(StepExecutionResult.Success(message));
        }
    }
}
