using FlowDesk.Abstractions;

namespace FlowDesk.App.ViewModels.Editor.PropertyEditors;

/// <summary>
/// 字符串属性编辑器。
/// </summary>
public sealed class StringPropertyEditorViewModel : PropertyEditorViewModel
{
    private string _value;

    public StringPropertyEditorViewModel(StepPropertyDescriptor descriptor, string initialValue)
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
