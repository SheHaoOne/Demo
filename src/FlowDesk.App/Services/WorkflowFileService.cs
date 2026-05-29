using FlowDesk.Abstractions;
using FlowDesk.Core;
using Microsoft.Win32;

namespace FlowDesk.App.Services;

public sealed class WorkflowFileService
{
    private readonly JsonWorkflowSerializer _serializer;

    public WorkflowFileService(JsonWorkflowSerializer serializer)
    {
        _serializer = serializer;
    }

    public async Task SaveAsync(WorkflowSequence sequence, CancellationToken cancellationToken = default)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "FlowDesk workflow (*.flow.json)|*.flow.json|JSON file (*.json)|*.json",
            FileName = $"{sequence.Name}.flow.json"
        };

        if (dialog.ShowDialog() == true)
        {
            await _serializer.SaveAsync(sequence, dialog.FileName, cancellationToken);
        }
    }

    public async Task<WorkflowSequence?> OpenAsync(CancellationToken cancellationToken = default)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "FlowDesk workflow (*.flow.json)|*.flow.json|JSON file (*.json)|*.json"
        };

        return dialog.ShowDialog() == true
            ? await _serializer.LoadAsync(dialog.FileName, cancellationToken)
            : null;
    }
}
