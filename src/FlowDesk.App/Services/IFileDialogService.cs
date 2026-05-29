namespace FlowDesk.App.Services;

/// <summary>
/// 文件对话框服务契约（DIP：ViewModel 不直接依赖 Win32 对话框）。
/// </summary>
public interface IFileDialogService
{
    string? ShowOpenDialog(string filter, string title = "打开文件");
    string? ShowSaveDialog(string filter, string defaultFileName = "", string title = "保存文件");
}
