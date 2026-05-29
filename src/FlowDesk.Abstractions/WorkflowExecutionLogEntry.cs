namespace FlowDesk.Abstractions;

public sealed record WorkflowExecutionLogEntry(
    DateTimeOffset Timestamp,
    string StepInstanceId,
    string StepDisplayName,
    bool Succeeded,
    string Message);
