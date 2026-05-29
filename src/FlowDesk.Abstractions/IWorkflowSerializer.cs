namespace FlowDesk.Abstractions;

/// <summary>
/// 工作流序列化/反序列化契约。
/// </summary>
public interface IWorkflowSerializer
{
    string Serialize(WorkflowSequence sequence);
    WorkflowSequence Deserialize(string data);
    Task SaveAsync(WorkflowSequence sequence, string path, CancellationToken cancellationToken = default);
    Task<WorkflowSequence> LoadAsync(string path, CancellationToken cancellationToken = default);
}
