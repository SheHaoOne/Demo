using FlowDesk.Abstractions;

namespace FlowDesk.Core;

public sealed class PluginCatalog : IPluginCatalog
{
    private readonly Dictionary<string, IWorkflowStepPlugin> _plugins = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<IWorkflowStepPlugin> Plugins => _plugins.Values
        .OrderBy(plugin => plugin.Descriptor.Category, StringComparer.OrdinalIgnoreCase)
        .ThenBy(plugin => plugin.Descriptor.DisplayName, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public void Register(IWorkflowStepPlugin plugin)
    {
        ArgumentNullException.ThrowIfNull(plugin);

        var id = plugin.Descriptor.Id;
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Plugin id cannot be empty.", nameof(plugin));
        }

        if (!_plugins.TryAdd(id, plugin))
        {
            throw new InvalidOperationException($"A workflow step plugin with id '{id}' is already registered.");
        }
    }

    public void RegisterRange(IEnumerable<IWorkflowStepPlugin> plugins)
    {
        foreach (var plugin in plugins)
        {
            Register(plugin);
        }
    }

    public bool TryGet(string pluginId, out IWorkflowStepPlugin plugin)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginId);
        return _plugins.TryGetValue(pluginId, out plugin!);
    }
}
