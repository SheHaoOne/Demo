using FlowDesk.Abstractions;

namespace FlowDesk.SamplePlugin;

public sealed class MessageStepPlugin : IWorkflowStepPlugin
{
    public PluginDescriptor Descriptor { get; } = new(
        "sample.message",
        "Message",
        "Samples",
        "将消息写入工作流执行日志。",
        "1.0.0");

    public IReadOnlyDictionary<string, string> DefaultSettings { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["message"] = "Hello from the workflow."
        };

    public IReadOnlyList<StepPropertyDescriptor> PropertyDescriptors { get; } =
    [
        new StepPropertyDescriptor
        {
            Name = "message",
            DisplayName = "消息内容",
            Description = "要写入执行日志的消息文本",
            Category = "常规",
            PropertyType = StepPropertyType.String,
            DefaultValue = "Hello from the workflow.",
            IsRequired = true
        }
    ];

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
