using FlowDesk.Abstractions;

namespace FlowDesk.App.ViewModels.Editor.PropertyEditors;

/// <summary>
/// 枚举属性编辑器（下拉选择）。
/// </summary>
public sealed class EnumPropertyEditorViewModel : PropertyEditorViewModel
{
    private string _value;

    public EnumPropertyEditorViewModel(StepPropertyDescriptor descriptor, string initialValue)
        : base(descriptor, initialValue)
    {
        _value = initialValue;
        Options = descriptor.EnumOptions ?? [];
    }

    public string[] Options { get; }

    public override string StringValue
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}
