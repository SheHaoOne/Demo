using FlowDesk.App.Services;
using FlowDesk.App.ViewModels.Config;
using FlowDesk.App.ViewModels.Editor;
using FlowDesk.App.ViewModels.Home;
using FlowDesk.App.ViewModels.Shell;
using FlowDesk.Core;
using FlowDesk.UI.Mvvm;
using FlowDesk.UI.Services;

namespace FlowDesk.App.Composition;

/// <summary>
/// 应用组合根（SRP：集中管理依赖创建和连接）。
/// </summary>
public static class AppCompositionRoot
{
    public static ShellViewModel Build()
    {
        // 基础设施
        var catalog = AppBootstrapper.CreatePluginCatalog();
        var runner = new WorkflowRunner(catalog);
        var executionService = new WorkflowExecutionService(runner);
        var themeService = new WpfThemeService();

        // 页面 ViewModel
        var homePage = new HomeViewModel(executionService);
        var configPage = new ConfigViewModel();
        var editorPage = new SequenceEditorViewModel(catalog);

        // 页面间联动：编辑器 → 首页
        editorPage.SequenceReady += (_, sequence) => homePage.LoadSequence(sequence);

        // 页面注册表（OCP：新增页面只需在此添加一行）
        var pages = new List<(string key, string label, string icon, ObservableObject page)>
        {
            ("home", "首页", "\uE80F", homePage),
            ("config", "配置", "\uE713", configPage),
            ("editor", "编辑器", "\uE70F", editorPage)
        };

        return new ShellViewModel(themeService, pages);
    }
}
