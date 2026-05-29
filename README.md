# FlowDesk

FlowDesk is a WPF desktop application skeleton for building plugin-driven workflow tools. Users compose a workflow sequence from registered step plugins, edit each step's settings, save or load the sequence as JSON, and run the sequence from the desktop shell.

## Project layout

- `src/FlowDesk.Abstractions` - stable contracts for workflow step plugins.
- `src/FlowDesk.Core` - plugin catalog, assembly loading, workflow execution, and JSON persistence.
- `src/FlowDesk.SamplePlugin` - built-in sample workflow steps.
- `src/FlowDesk.App` - WPF MVVM desktop shell and sequence editor.
- `tests/FlowDesk.Core.Tests` - dependency-free executable checks for the core workflow behavior.

## Build and test

Install the .NET 8 SDK, then run:

- `dotnet build FlowDesk.sln`
- `dotnet run --project tests/FlowDesk.Core.Tests/FlowDesk.Core.Tests.csproj`

`FlowDesk.App` targets `net8.0-windows` with WPF enabled. The project sets `EnableWindowsTargeting=true` so it can be compiled on non-Windows build agents, while running the desktop application still requires Windows.

## Plugin model

Workflow plugins implement `IWorkflowStepPlugin` and return an `IWorkflowStepExecutor`. Drop plugin assemblies into a `Plugins` directory next to the application executable to load them at startup.