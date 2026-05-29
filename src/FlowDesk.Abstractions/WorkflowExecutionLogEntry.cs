namespace FlowDesk.Abstractions;

/// <summary>
/// 工作流执行日志条目。
/// </summary>
public sealed record WorkflowExecutionLogEntry(
    DateTimeOffset Timestamp,
    string StepInstanceId,
    string StepDisplayName,
    bool Succeeded,
    string Message)
{
    /// <summary>
    /// 执行附加数据（如 TDMS 文件数据），用于 UI 展示图表等。
    /// </summary>
    public IDictionary<string, object?> Attachments { get; init; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
