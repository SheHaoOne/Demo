using System.Windows;

namespace FlowDesk.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // 主题初始化统一由 AppCompositionRoot 处理，此处不再重复调用
    }
}
