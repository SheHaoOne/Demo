# AGENTS

## Cursor Cloud specific instructions

This repository contains a .NET 8 WPF desktop application skeleton named FlowDesk.

### Tech stack

- .NET 8 SDK
- WPF desktop application targeting `net8.0-windows`
- SDK-style C# projects with nullable reference types enabled
- No third-party NuGet dependencies

### Project layout

- `src/FlowDesk.Abstractions` - plugin contracts and workflow data models.
- `src/FlowDesk.Core` - plugin catalog, assembly loader, workflow runner, and JSON serializer.
- `src/FlowDesk.SamplePlugin` - built-in sample workflow step plugins.
- `src/FlowDesk.App` - WPF MVVM desktop shell.
- `tests/FlowDesk.Core.Tests` - dependency-free executable test project.

### Build and test

Use the .NET SDK commands below:

- `dotnet build FlowDesk.sln`
- `dotnet run --project tests/FlowDesk.Core.Tests/FlowDesk.Core.Tests.csproj`

`FlowDesk.App` sets `EnableWindowsTargeting=true`, so it can compile on Linux build agents. Running the WPF application still requires Windows.
