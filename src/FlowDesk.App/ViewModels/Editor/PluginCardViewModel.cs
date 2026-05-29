using FlowDesk.Abstractions;

namespace FlowDesk.App.ViewModels.Editor;

/// <summary>
/// 单个插件卡片 ViewModel。
/// </summary>
public sealed class PluginCardViewModel
{
    public PluginCardViewModel(IWorkflowStepPlugin plugin)
    {
        Plugin = plugin;
    }

    public IWorkflowStepPlugin Plugin { get; }
    public string Id => Plugin.Descriptor.Id;
    public string DisplayName => Plugin.Descriptor.DisplayName;
    public string Category => Plugin.Descriptor.Category;
    public string Description => Plugin.Descriptor.Description;
    public string Version => Plugin.Descriptor.Version;
}
