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
- `src/FlowDesk.UI` - shared WPF styles, resource dictionaries, and MVVM infrastructure (`ObservableObject`, `RelayCommand`).
- `src/FlowDesk.App` - WPF MVVM desktop shell (references `FlowDesk.UI` for styles and MVVM base classes).
- `tests/FlowDesk.Core.Tests` - dependency-free executable test project.

### Build and test

Use the .NET SDK commands below:

- `dotnet build FlowDesk.sln`
- `dotnet run --project tests/FlowDesk.Core.Tests/FlowDesk.Core.Tests.csproj`

`FlowDesk.App` sets `EnableWindowsTargeting=true`, so it can compile on Linux build agents. Running the WPF application still requires Windows.

On Linux agents, use the official Microsoft .NET SDK distribution rather than distro-packaged SDKs if WPF compilation fails with a missing `Microsoft.NET.Sdk.WindowsDesktop` target.

### Cloud VM notes

- The .NET 8 SDK is installed to `$HOME/.dotnet`. The update script exports `DOTNET_ROOT` and `PATH` automatically; if you open a new shell, source `~/.bashrc` or set `export PATH="$HOME/.dotnet:$PATH"`.
- The test project (`FlowDesk.Core.Tests`) is a plain console app, not an xUnit/NUnit/MSTest project. Run it with `dotnet run`, not `dotnet test`.
- The WPF app (`FlowDesk.App`) compiles on Linux but cannot launch a GUI window. Validate it via `dotnet build` only on Linux agents.
