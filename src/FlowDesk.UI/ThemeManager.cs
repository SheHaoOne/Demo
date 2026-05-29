using System.Windows;

namespace FlowDesk.UI;

/// <summary>
/// 应用主题类型。
/// </summary>
public enum AppTheme
{
    Light,
    Dark
}

/// <summary>
/// 运行时主题管理器，支持亮色/深色主题一键切换。
/// 调用 <see cref="ApplyTheme"/> 即可在运行时切换整个应用的 UI 风格。
/// </summary>
public static class ThemeManager
{
    private static readonly Uri LightThemeUri =
        new("pack://application:,,,/FlowDesk.UI;component/Themes/Light.xaml", UriKind.Absolute);

    private static readonly Uri DarkThemeUri =
        new("pack://application:,,,/FlowDesk.UI;component/Themes/Dark.xaml", UriKind.Absolute);

    private static ResourceDictionary? _currentThemeDictionary;
    private static AppTheme _currentTheme = AppTheme.Light;

    /// <summary>
    /// 当前生效的主题。
    /// </summary>
    public static AppTheme CurrentTheme => _currentTheme;

    /// <summary>
    /// 切换到指定主题。会替换 Application.Resources 中的主题资源字典。
    /// </summary>
    public static void ApplyTheme(AppTheme theme)
    {
        var app = Application.Current;
        if (app is null) return;

        var mergedDicts = app.Resources.MergedDictionaries;

        if (_currentThemeDictionary is not null)
        {
            mergedDicts.Remove(_currentThemeDictionary);
        }

        var uri = theme == AppTheme.Dark ? DarkThemeUri : LightThemeUri;
        _currentThemeDictionary = new ResourceDictionary { Source = uri };
        mergedDicts.Insert(0, _currentThemeDictionary);

        _currentTheme = theme;
    }

    /// <summary>
    /// 在亮色和深色之间切换。
    /// </summary>
    public static void ToggleTheme()
    {
        ApplyTheme(_currentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light);
    }
}
