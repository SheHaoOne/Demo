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
}
