using FlowDesk.Abstractions;
using FlowDesk.Core;

var tests = new (string Name, Func<Task> Run)[]
{
    ("Plugin catalog rejects duplicate ids", PluginCatalogRejectsDuplicateIds),
    ("Workflow runner executes steps in order", WorkflowRunnerExecutesStepsInOrder),
    ("Workflow runner stops after failure", WorkflowRunnerStopsAfterFailure),
    ("JSON serializer round trips workflow sequences", JsonSerializerRoundTripsWorkflowSequences),
    ("TdmsFileReader generates demo data for missing file", TdmsFileReaderGeneratesDemoDataForMissingFile),
    ("TdmsFileReader throws on null path", TdmsFileReaderThrowsOnNullPath),
    ("WorkflowRunner attaches tdmsData to log entry", WorkflowRunnerAttachesTdmsDataToLogEntry)
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

static Task TdmsFileReaderGeneratesDemoDataForMissingFile()
{
    var reader = new TdmsFileReader();
    var data = reader.Read("/tmp/__nonexistent__.tdms");
    AssertEqual(true, data.Channels.Count > 0, "应返回演示数据通道");
    AssertEqual("/tmp/__nonexistent__.tdms", data.FilePath, "文件路径");
    return Task.CompletedTask;
}

static Task TdmsFileReaderThrowsOnNullPath()
{
    var reader = new TdmsFileReader();
    AssertThrows<ArgumentException>(() => reader.Read(""));
    AssertThrows<ArgumentException>(() => reader.Read("   "));
    return Task.CompletedTask;
}

static async Task WorkflowRunnerAttachesTdmsDataToLogEntry()
{
    var catalog = new PluginCatalog();
    catalog.Register(new TestPlugin("test.tdms", context =>
    {
        // 模拟 DataImportStepPlugin 的行为：将 TdmsFileData 写入 variables
        var data = new TdmsFileData("test.tdms",
        [
            new TdmsChannelData("Group1", "Ch1", [1.0, 2.0, 3.0])
        ]);
        context.Variables["tdmsData"] = data;
        return StepExecutionResult.Success("OK");
    }));

    var sequence = new WorkflowSequence
    {
        Name = "TDMS attachment",
        Steps = [CreateStep("test.tdms", "Import")]
    };

    var entries = await new WorkflowRunner(catalog).RunAsync(sequence);

    AssertEqual(1, entries.Count, "Entry count");
    AssertEqual(true, entries[0].Attachments.ContainsKey("tdmsData"), "Has tdmsData attachment");
    var attached = entries[0].Attachments["tdmsData"] as TdmsFileData;
    AssertEqual(true, attached is not null, "Attachment is TdmsFileData");
    AssertEqual(1, attached!.Channels.Count, "Channel count");
    AssertEqual("Group1/Ch1", attached.Channels[0].DisplayName, "Channel display name");
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

sealed class TestPlugin : IWorkflowStepPlugin
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

    public IReadOnlyList<StepPropertyDescriptor> PropertyDescriptors { get; } = [];

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
