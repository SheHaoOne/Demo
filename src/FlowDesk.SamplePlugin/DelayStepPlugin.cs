using FlowDesk.Abstractions;

namespace FlowDesk.SamplePlugin;

public sealed class DelayStepPlugin : IWorkflowStepPlugin
{
    public PluginDescriptor Descriptor { get; } = new(
        "sample.delay",
        "Delay",
        "Samples",
        "Waits for a configured number of milliseconds.",
        "1.0.0");

    public IReadOnlyDictionary<string, string> DefaultSettings { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["milliseconds"] = "500"
        };

    public IWorkflowStepExecutor CreateExecutor() => new Executor();

    private sealed class Executor : IWorkflowStepExecutor
    {
        public async ValueTask<StepExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken)
        {
            var configuredValue = context.Step.Settings.TryGetValue("milliseconds", out var value)
                ? value
                : "500";

            if (!int.TryParse(configuredValue, out var milliseconds) || milliseconds < 0)
            {
                return StepExecutionResult.Failure("Delay milliseconds must be a non-negative integer.");
            }

            await Task.Delay(milliseconds, cancellationToken);
            return StepExecutionResult.Success($"Waited {milliseconds} ms.");
        }
    }
}
