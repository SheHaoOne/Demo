using FlowDesk.Abstractions;

namespace FlowDesk.SamplePlugin;

/// <summary>
/// 数据导入步骤插件：从指定文件路径导入数据。
/// </summary>
public sealed class DataImportStepPlugin : IWorkflowStepPlugin
{
    public PluginDescriptor Descriptor { get; } = new(
        "daq.import",
        "数据导入",
        "数据采集",
        "从指定文件路径导入数据到工作流变量。",
        "1.0.0");

    public IReadOnlyDictionary<string, string> DefaultSettings { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["filePath"] = "",
            ["format"] = "CSV",
            ["hasHeader"] = "True"
        };

    public IReadOnlyList<StepPropertyDescriptor> PropertyDescriptors { get; } =
    [
        new StepPropertyDescriptor
        {
            Name = "filePath",
            DisplayName = "文件路径",
            Description = "要导入的数据文件完整路径",
            Category = "数据源",
            PropertyType = StepPropertyType.FilePath,
            DefaultValue = "",
            IsRequired = true
        },
        new StepPropertyDescriptor
        {
            Name = "format",
            DisplayName = "文件格式",
            Description = "数据文件的格式类型",
            Category = "数据源",
            PropertyType = StepPropertyType.Enum,
            DefaultValue = "CSV",
            EnumOptions = ["CSV", "JSON", "XML", "Excel"],
            IsRequired = true
        },
        new StepPropertyDescriptor
        {
            Name = "hasHeader",
            DisplayName = "包含表头",
            Description = "数据文件第一行是否为列标题",
            Category = "解析选项",
            PropertyType = StepPropertyType.Boolean,
            DefaultValue = true
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

            var filePath = context.Step.Settings.TryGetValue("filePath", out var fp) ? fp : "";
            var format = context.Step.Settings.TryGetValue("format", out var fmt) ? fmt : "CSV";

            if (string.IsNullOrWhiteSpace(filePath))
                return ValueTask.FromResult(StepExecutionResult.Failure("文件路径不能为空。"));

            context.Variables["importedFilePath"] = filePath;
            context.Variables["importedFormat"] = format;

            return ValueTask.FromResult(
                StepExecutionResult.Success($"已导入数据文件：{filePath}（格式：{format}）"));
        }
    }
}
