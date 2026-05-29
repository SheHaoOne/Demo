using System.Collections.ObjectModel;

namespace FlowDesk.App.ViewModels.Editor;

/// <summary>
/// 插件分类（卡片组）。
/// </summary>
public sealed class PluginCategoryViewModel
{
    public PluginCategoryViewModel(string category, ObservableCollection<PluginCardViewModel> plugins)
    {
        Category = category;
        Plugins = plugins;
    }

    public string Category { get; }
    public ObservableCollection<PluginCardViewModel> Plugins { get; }

    /// <summary>
    /// 分类图标（使用 Segoe MDL2 Assets 字符）。
    /// </summary>
    public string Icon => Category.ToUpperInvariant() switch
    {
        "SAMPLES" => "\uE8B7",
        "数据采集" => "\uE9F5",
        "MEASUREMENT" => "\uE9D9",
        "COMMUNICATION" => "\uE774",
        "CONTROL" => "\uE7FC",
        _ => "\uE737"
    };
}
