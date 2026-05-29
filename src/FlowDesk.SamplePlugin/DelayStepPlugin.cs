using FlowDesk.Abstractions;

namespace FlowDesk.SamplePlugin;

public sealed class DelayStepPlugin : IWorkflowStepPlugin
{
    public PluginDescriptor Descriptor { get; } = new(
        "sample.delay",
        "Delay",
        "Samples",
        "等待指定的毫秒数。",
        "1.0.0");

    public IReadOnlyDictionary<string, string> DefaultSettings { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["milliseconds"] = "500"
        };

    public IReadOnlyList<StepPropertyDescriptor> PropertyDescriptors { get; } =
    [
        new StepPropertyDescriptor
        {
            Name = "milliseconds",
            DisplayName = "延迟时间",
            Description = "等待的毫秒数",
            Category = "常规",
            PropertyType = StepPropertyType.Integer,
            DefaultValue = 500,
            MinValue = 0,
            MaxValue = 60000,
            IsRequired = true
        }
    ];

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
                return StepExecutionResult.Failure("延迟毫秒数必须为非负整数。");
            }

            await Task.Delay(milliseconds, cancellationToken);
            return StepExecutionResult.Success($"等待了 {milliseconds} ms。");
        }
    }
}
