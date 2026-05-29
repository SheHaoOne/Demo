namespace FlowDesk.Abstractions;

public interface IWorkflowStepPlugin
{
    PluginDescriptor Descriptor { get; }

    IReadOnlyDictionary<string, string> DefaultSettings { get; }

    IWorkflowStepExecutor CreateExecutor();
}
