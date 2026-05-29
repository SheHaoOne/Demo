using Microsoft.Win32;

namespace FlowDesk.App.Services;

/// <summary>
/// WPF 文件对话框服务实现。
/// </summary>
public sealed class WpfFileDialogService : IFileDialogService
{
    public string? ShowOpenDialog(string filter, string title = "打开文件")
    {
        var dialog = new OpenFileDialog { Filter = filter, Title = title };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? ShowSaveDialog(string filter, string defaultFileName = "", string title = "保存文件")
    {
        var dialog = new SaveFileDialog { Filter = filter, FileName = defaultFileName, Title = title };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
