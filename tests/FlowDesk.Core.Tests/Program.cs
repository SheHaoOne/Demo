using FlowDesk.Abstractions;
using FlowDesk.Core;

var tests = new (string Name, Func<Task> Run)[]
{
    ("Plugin catalog rejects duplicate ids", PluginCatalogRejectsDuplicateIds),
    ("Workflow runner executes steps in order", WorkflowRunnerExecutesStepsInOrder),
    ("Workflow runner stops after failure", WorkflowRunnerStopsAfterFailure),
    ("JSON serializer round trips workflow sequences", JsonSerializerRoundTripsWorkflowSequences)
};

foreach (var test in tests)
{
    await test.Run();
    Console.WriteLine($"PASS {test.Name}");
}

return 0;

static Task PluginCatalogRejectsDuplicateIds()
{
    var catalog = new PluginCatalog();
    catalog.Register(new TestPlugin("test.same", _ => StepExecutionResult.Success("first")));

    AssertThrows<InvalidOperationException>(() =>
        catalog.Register(new TestPlugin("test.same", _ => StepExecutionResult.Success("second"))));

    return Task.CompletedTask;
}

static async Task WorkflowRunnerExecutesStepsInOrder()
{
    var executedSteps = new List<string>();
    var catalog = new PluginCatalog();
    catalog.Register(new TestPlugin("test.capture", context =>
    {
        executedSteps.Add(context.Step.DisplayName);
        return StepExecutionResult.Success(context.Step.DisplayName);
    }));

    var sequence = new WorkflowSequence
    {
        Name = "Execution order",
        Steps =
        [
            CreateStep("test.capture", "First"),
            CreateStep("test.capture", "Second")
        ]
    };

    var entries = await new WorkflowRunner(catalog).RunAsync(sequence);

    AssertEqual(2, entries.Count, "Execution log count");
    AssertEqual("First,Second", string.Join(",", executedSteps), "Execution order");
}

static async Task WorkflowRunnerStopsAfterFailure()
{
    var catalog = new PluginCatalog();
    catalog.Register(new TestPlugin("test.ok", _ => StepExecutionResult.Success("ok")));
    catalog.Register(new TestPlugin("test.fail", _ => StepExecutionResult.Failure("failed")));

    var sequence = new WorkflowSequence
    {
        Name = "Stops on failure",
        Steps =
        [
            CreateStep("test.ok", "First"),
            CreateStep("test.fail", "Failure"),
            CreateStep("test.ok", "Skipped")
        ]
    };

    var entries = await new WorkflowRunner(catalog).RunAsync(sequence);

    AssertEqual(2, entries.Count, "Execution log count");
    AssertEqual(false, entries[^1].Succeeded, "Failure state");
    AssertEqual("Failure", entries[^1].StepDisplayName, "Failed step");
}

static Task JsonSerializerRoundTripsWorkflowSequences()
{
    var serializer = new JsonWorkflowSerializer();
    var sequence = new WorkflowSequence
    {
        Name = "Round trip",
        Steps =
        [
            new WorkflowStepDefinition
            {
                InstanceId = "step-1",
                PluginId = "sample.message",
                DisplayName = "Message",
                Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["message"] = "Persist me"
                }
            }
        ]
    };

    var copy = serializer.Deserialize(serializer.Serialize(sequence));

    AssertEqual(sequence.Name, copy.Name, "Sequence name");
    AssertEqual(sequence.Steps[0].PluginId, copy.Steps[0].PluginId, "Plugin id");
    AssertEqual("Persist me", copy.Steps[0].Settings["message"], "Setting value");
    return Task.CompletedTask;
}

static WorkflowStepDefinition CreateStep(string pluginId, string displayName)
{
    return new WorkflowStepDefinition
    {
        PluginId = pluginId,
        DisplayName = displayName
    };
}

static void AssertEqual<T>(T expected, T actual, string message)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"{message}: expected '{expected}', actual '{actual}'.");
    }
}

static void AssertThrows<TException>(Action action)
    where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException($"Expected exception '{typeof(TException).Name}' was not thrown.");
}

private sealed class TestPlugin : IWorkflowStepPlugin
{
    private readonly Func<WorkflowExecutionContext, StepExecutionResult> _execute;

    public TestPlugin(string id, Func<WorkflowExecutionContext, StepExecutionResult> execute)
    {
        _execute = execute;
        Descriptor = new PluginDescriptor(id, id, "Tests", "Test plugin", "1.0.0");
    }

    public PluginDescriptor Descriptor { get; }

    public IReadOnlyDictionary<string, string> DefaultSettings { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public IWorkflowStepExecutor CreateExecutor() => new TestExecutor(_execute);

    private sealed class TestExecutor : IWorkflowStepExecutor
    {
        private readonly Func<WorkflowExecutionContext, StepExecutionResult> _execute;

        public TestExecutor(Func<WorkflowExecutionContext, StepExecutionResult> execute)
        {
            _execute = execute;
        }

        public ValueTask<StepExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(_execute(context));
        }
    }
}
