namespace FlowDesk.Abstractions;

/// <summary>
/// 工作流步骤分组。
/// </summary>
public sealed class WorkflowStepGroup
{
    public string GroupId { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "新建分组";
    public List<WorkflowStepDefinition> Steps { get; set; } = [];
}
