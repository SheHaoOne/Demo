namespace FlowDesk.UI.Services;

/// <summary>
/// 主题服务契约（DIP：面向接口而非静态类）。
/// </summary>
public interface IThemeService
{
    FlowDesk.UI.Themes.AppTheme CurrentTheme { get; }
    void ApplyTheme(FlowDesk.UI.Themes.AppTheme theme);
    void ToggleTheme();
}
