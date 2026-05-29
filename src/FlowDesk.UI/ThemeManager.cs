using System.Windows;
using FlowDesk.UI.Themes;

namespace FlowDesk.UI;

/// <summary>
/// 静态便捷入口（向后兼容）。新代码应优先注入 <see cref="FlowDesk.UI.Services.IThemeService"/>。
/// </summary>
public static class ThemeManager
{
    private static readonly Uri LightThemeUri =
        new("pack://application:,,,/FlowDesk.UI;component/Themes/Light.xaml", UriKind.Absolute);

    private static readonly Uri DarkThemeUri =
        new("pack://application:,,,/FlowDesk.UI;component/Themes/Dark.xaml", UriKind.Absolute);

    private static ResourceDictionary? _currentThemeDictionary;
    private static AppTheme _currentTheme = AppTheme.Light;

    public static AppTheme CurrentTheme => _currentTheme;

    public static void ApplyTheme(AppTheme theme)
    {
        var app = Application.Current;
        if (app is null) return;

        var mergedDicts = app.Resources.MergedDictionaries;
        if (_currentThemeDictionary is not null)
            mergedDicts.Remove(_currentThemeDictionary);

        var uri = theme == AppTheme.Dark ? DarkThemeUri : LightThemeUri;
        _currentThemeDictionary = new ResourceDictionary { Source = uri };
        mergedDicts.Add(_currentThemeDictionary);
        _currentTheme = theme;
    }

    public static void ToggleTheme()
    {
        ApplyTheme(_currentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light);
    }
}
