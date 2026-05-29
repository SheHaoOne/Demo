namespace FlowDesk.Abstractions;

public sealed record StepExecutionResult(bool Succeeded, string Message)
{
    public static StepExecutionResult Success(string message) => new(true, message);

    public static StepExecutionResult Failure(string message) => new(false, message);
}
