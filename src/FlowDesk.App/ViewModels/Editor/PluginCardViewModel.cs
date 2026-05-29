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

    /// <summary>
    /// 步骤图标（使用 Segoe MDL2 Assets 字符）。
    /// </summary>
    public string Icon => Plugin.Descriptor.Category.ToUpperInvariant() switch
    {
        "SAMPLES" => "\uE8B7",
        "数据采集" => "\uE9F5",
        "MEASUREMENT" => "\uE9D9",
        "COMMUNICATION" => "\uE774",
        "CONTROL" => "\uE7FC",
        _ => "\uE737"
    };
}
