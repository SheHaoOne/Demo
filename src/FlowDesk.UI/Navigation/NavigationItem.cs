using FlowDesk.UI.Mvvm;

namespace FlowDesk.UI.Navigation;

/// <summary>
/// 导航菜单项数据模型。
/// </summary>
public sealed class NavigationItem : ObservableObject
{
    private bool _isSelected;

    public NavigationItem(string key, string label, string icon)
    {
        Key = key;
        Label = label;
        Icon = icon;
    }

    /// <summary>页面唯一标识。</summary>
    public string Key { get; }

    /// <summary>显示文本。</summary>
    public string Label { get; }

    /// <summary>Unicode 图标字符（用于 Segoe Fluent Icons 或文字替代）。</summary>
    public string Icon { get; }

    /// <summary>是否为当前选中项。</summary>
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
