using FlowDesk.Abstractions;

namespace FlowDesk.App.ViewModels;

public sealed class PluginViewModel
{
    public PluginViewModel(IWorkflowStepPlugin plugin)
    {
        Plugin = plugin;
    }

    public IWorkflowStepPlugin Plugin { get; }

    public string Id => Plugin.Descriptor.Id;

    public string DisplayName => Plugin.Descriptor.DisplayName;

    public string Category => Plugin.Descriptor.Category;

    public string Description => Plugin.Descriptor.Description;
}
