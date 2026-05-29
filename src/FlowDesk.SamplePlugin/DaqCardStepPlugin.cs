using FlowDesk.Abstractions;

namespace FlowDesk.SamplePlugin;

/// <summary>
/// 采集卡采集步骤插件：通过采集卡设备进行数据采集。
/// </summary>
public sealed class DaqCardStepPlugin : IWorkflowStepPlugin
{
    public PluginDescriptor Descriptor { get; } = new(
        "daq.card",
        "采集卡采集",
        "数据采集",
        "通过指定采集卡设备进行模拟信号数据采集。",
        "1.0.0");

    public IReadOnlyDictionary<string, string> DefaultSettings { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["deviceName"] = "Dev1",
            ["channelCount"] = "4",
            ["sampleRate"] = "1000",
            ["duration"] = "1.0",
            ["voltageRange"] = "10"
        };

    public IReadOnlyList<StepPropertyDescriptor> PropertyDescriptors { get; } =
    [
        new StepPropertyDescriptor
        {
            Name = "deviceName",
            DisplayName = "设备名称",
            Description = "采集卡设备标识符",
            Category = "设备",
            PropertyType = StepPropertyType.String,
            DefaultValue = "Dev1",
            IsRequired = true
        },
        new StepPropertyDescriptor
        {
            Name = "channelCount",
            DisplayName = "通道数",
            Description = "采集通道数量",
            Category = "设备",
            PropertyType = StepPropertyType.Integer,
            DefaultValue = 4,
            MinValue = 1,
            MaxValue = 64,
            IsRequired = true
        },
        new StepPropertyDescriptor
        {
            Name = "sampleRate",
            DisplayName = "采样率 (Hz)",
            Description = "每秒采样次数",
            Category = "采集参数",
            PropertyType = StepPropertyType.Integer,
            DefaultValue = 1000,
            MinValue = 1,
            MaxValue = 1000000,
            IsRequired = true
        },
        new StepPropertyDescriptor
        {
            Name = "duration",
            DisplayName = "采集时长 (秒)",
            Description = "持续采集的时间长度",
            Category = "采集参数",
            PropertyType = StepPropertyType.Double,
            DefaultValue = 1.0,
            MinValue = 0.001,
            MaxValue = 3600.0,
            IsRequired = true
        },
        new StepPropertyDescriptor
        {
            Name = "voltageRange",
            DisplayName = "量程 (V)",
            Description = "模拟输入电压量程",
            Category = "采集参数",
            PropertyType = StepPropertyType.Enum,
            DefaultValue = "10",
            EnumOptions = ["0.1", "0.5", "1", "5", "10", "20"],
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
            var device = context.Step.Settings.TryGetValue("deviceName", out var d) ? d : "Dev1";
            var rateStr = context.Step.Settings.TryGetValue("sampleRate", out var r) ? r : "1000";
            var durationStr = context.Step.Settings.TryGetValue("duration", out var dur) ? dur : "1.0";
            var channelsStr = context.Step.Settings.TryGetValue("channelCount", out var ch) ? ch : "4";

            if (!int.TryParse(rateStr, out var sampleRate) || sampleRate <= 0)
                return StepExecutionResult.Failure("采样率必须为正整数。");

            if (!double.TryParse(durationStr, System.Globalization.CultureInfo.InvariantCulture, out var duration) || duration <= 0)
                return StepExecutionResult.Failure("采集时长必须为正数。");

            var delayMs = (int)(duration * 1000);
            await Task.Delay(Math.Min(delayMs, 5000), cancellationToken);

            var totalSamples = (long)(sampleRate * duration);
            context.Variables["daqSamples"] = totalSamples;
            context.Variables["daqDevice"] = device;

            return StepExecutionResult.Success(
                $"采集完成：设备 {device}，{channelsStr} 通道，{sampleRate} Hz × {duration:F1} 秒 = {totalSamples} 样本");
        }
    }
}
