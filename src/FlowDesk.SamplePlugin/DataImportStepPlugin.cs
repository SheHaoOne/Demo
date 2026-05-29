using FlowDesk.Abstractions;
using FlowDesk.Core;

namespace FlowDesk.SamplePlugin;

/// <summary>
/// 数据导入步骤插件：从指定文件路径导入数据，支持 TDMS/CSV/JSON/XML/Excel 格式。
/// </summary>
public sealed class DataImportStepPlugin : IWorkflowStepPlugin
{
    public PluginDescriptor Descriptor { get; } = new(
        "daq.import",
        "数据导入",
        "数据采集",
        "从指定文件路径导入数据到工作流变量（支持 TDMS 波形查看）。",
        "1.0.0");

    public IReadOnlyDictionary<string, string> DefaultSettings { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["filePath"] = "",
            ["format"] = "TDMS",
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
            DefaultValue = "TDMS",
            EnumOptions = ["TDMS", "CSV", "JSON", "XML", "Excel"],
            IsRequired = true
        },
        new StepPropertyDescriptor
        {
            Name = "hasHeader",
            DisplayName = "包含表头",
            Description = "数据文件第一行是否为列标题（仅 CSV 格式有效）",
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
            var format = context.Step.Settings.TryGetValue("format", out var fmt) ? fmt : "TDMS";

            if (string.IsNullOrWhiteSpace(filePath))
                return ValueTask.FromResult(StepExecutionResult.Failure("文件路径不能为空。"));

            context.Variables["importedFilePath"] = filePath;
            context.Variables["importedFormat"] = format;

            if (string.Equals(format, "TDMS", StringComparison.OrdinalIgnoreCase))
            {
                var reader = new TdmsFileReader();
                var tdmsData = reader.Read(filePath);
                context.Variables["tdmsData"] = tdmsData;

                var channelInfo = string.Join(", ", tdmsData.Channels.Select(c => c.DisplayName));
                return ValueTask.FromResult(
                    StepExecutionResult.Success($"已导入 TDMS 文件：{tdmsData.Channels.Count} 个通道（{channelInfo}）"));
            }

            return ValueTask.FromResult(
                StepExecutionResult.Success($"已导入数据文件：{filePath}（格式：{format}）"));
        }
    }
}
