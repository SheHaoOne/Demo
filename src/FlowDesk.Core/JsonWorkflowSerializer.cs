using System.Text.Json;
using FlowDesk.Abstractions;

namespace FlowDesk.Core;

public sealed class JsonWorkflowSerializer : IWorkflowSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public string Serialize(WorkflowSequence sequence)
    {
        ArgumentNullException.ThrowIfNull(sequence);
        return JsonSerializer.Serialize(sequence, Options);
    }

    public WorkflowSequence Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        return JsonSerializer.Deserialize<WorkflowSequence>(json, Options)
            ?? throw new InvalidOperationException("Workflow sequence JSON is empty.");
    }

    public async Task SaveAsync(
        WorkflowSequence sequence,
        string path,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sequence);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await File.WriteAllTextAsync(path, Serialize(sequence), cancellationToken);
    }

    public async Task<WorkflowSequence> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var json = await File.ReadAllTextAsync(path, cancellationToken);
        return Deserialize(json);
    }
}
