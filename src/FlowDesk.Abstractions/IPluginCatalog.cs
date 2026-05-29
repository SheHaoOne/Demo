namespace FlowDesk.Abstractions;

/// <summary>
/// 插件注册与查询目录契约。
/// </summary>
public interface IPluginCatalog
{
    IReadOnlyCollection<IWorkflowStepPlugin> Plugins { get; }
    void Register(IWorkflowStepPlugin plugin);
    void RegisterRange(IEnumerable<IWorkflowStepPlugin> plugins);
    bool TryGet(string pluginId, out IWorkflowStepPlugin plugin);
}
