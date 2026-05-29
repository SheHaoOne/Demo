namespace FlowDesk.Abstractions;

public sealed class WorkflowStepDefinition
{
    public string InstanceId { get; set; } = Guid.NewGuid().ToString("N");

    public string PluginId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public Dictionary<string, string> Settings { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
