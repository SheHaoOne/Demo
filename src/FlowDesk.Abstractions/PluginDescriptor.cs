namespace FlowDesk.Abstractions;

public sealed record PluginDescriptor(
    string Id,
    string DisplayName,
    string Category,
    string Description,
    string Version);
