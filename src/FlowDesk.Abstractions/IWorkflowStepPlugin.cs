namespace FlowDesk.Abstractions;

public interface IWorkflowStepPlugin
{
    PluginDescriptor Descriptor { get; }

    IReadOnlyDictionary<string, string> DefaultSettings { get; }

    /// <summary>
    /// 步骤属性描述符列表，用于属性编辑器渲染。
    /// </summary>
    IReadOnlyList<StepPropertyDescriptor> PropertyDescriptors { get; }

    IWorkflowStepExecutor CreateExecutor();
}
