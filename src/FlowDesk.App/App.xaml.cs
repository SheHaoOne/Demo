using System.Windows;
using FlowDesk.UI;

namespace FlowDesk.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ThemeManager.ApplyTheme(AppTheme.Light);
    }
}
