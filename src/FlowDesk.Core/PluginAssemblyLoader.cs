using System.Reflection;
using System.Runtime.Loader;
using FlowDesk.Abstractions;

namespace FlowDesk.Core;

public sealed class PluginAssemblyLoader
{
    public IReadOnlyCollection<IWorkflowStepPlugin> LoadFromDirectory(string pluginDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginDirectory);

        if (!Directory.Exists(pluginDirectory))
        {
            return [];
        }

        var plugins = new List<IWorkflowStepPlugin>();
        foreach (var assemblyPath in Directory.EnumerateFiles(pluginDirectory, "*.dll", SearchOption.TopDirectoryOnly))
        {
            plugins.AddRange(LoadFromAssembly(assemblyPath));
        }

        return plugins;
    }

    public IReadOnlyCollection<IWorkflowStepPlugin> LoadFromAssembly(string assemblyPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyPath);

        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(assemblyPath));
        var pluginTypes = GetLoadableTypes(assembly)
            .Where(type =>
                !type.IsAbstract &&
                type.GetConstructor(Type.EmptyTypes) is not null &&
                typeof(IWorkflowStepPlugin).IsAssignableFrom(type));

        return pluginTypes
            .Select(type => (IWorkflowStepPlugin)Activator.CreateInstance(type)!)
            .ToArray();
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type is not null)!;
        }
    }
}
