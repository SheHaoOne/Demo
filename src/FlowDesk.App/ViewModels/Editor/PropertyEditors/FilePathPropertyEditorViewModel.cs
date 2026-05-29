using FlowDesk.Abstractions;

namespace FlowDesk.App.ViewModels.Editor.PropertyEditors;

/// <summary>
/// 文件路径属性编辑器。
/// </summary>
public sealed class FilePathPropertyEditorViewModel : PropertyEditorViewModel
{
    private string _value;

    public FilePathPropertyEditorViewModel(StepPropertyDescriptor descriptor, string initialValue)
        : base(descriptor, initialValue)
    {
        _value = initialValue;
    }

    public override string StringValue
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}
