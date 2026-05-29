using System.Windows;
using FlowDesk.UI.Themes;

namespace FlowDesk.UI.Services;

/// <summary>
/// WPF 主题服务实现，通过替换 Application.Resources 中的资源字典实现主题切换。
/// </summary>
public sealed class WpfThemeService : IThemeService
{
    private static readonly Uri LightThemeUri =
        new("pack://application:,,,/FlowDesk.UI;component/Themes/Light.xaml", UriKind.Absolute);

    private static readonly Uri DarkThemeUri =
        new("pack://application:,,,/FlowDesk.UI;component/Themes/Dark.xaml", UriKind.Absolute);

    private ResourceDictionary? _currentThemeDictionary;

    public AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

    public void ApplyTheme(AppTheme theme)
    {
        var app = Application.Current;
        if (app is null) return;

        var mergedDicts = app.Resources.MergedDictionaries;

        if (_currentThemeDictionary is not null)
            mergedDicts.Remove(_currentThemeDictionary);

        var uri = theme == AppTheme.Dark ? DarkThemeUri : LightThemeUri;
        _currentThemeDictionary = new ResourceDictionary { Source = uri };
        mergedDicts.Add(_currentThemeDictionary);

        CurrentTheme = theme;
    }

    public void ToggleTheme()
    {
        ApplyTheme(CurrentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light);
    }
}
