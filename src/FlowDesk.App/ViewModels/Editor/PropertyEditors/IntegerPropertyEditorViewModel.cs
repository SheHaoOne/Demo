using FlowDesk.Abstractions;

namespace FlowDesk.App.ViewModels.Editor.PropertyEditors;

/// <summary>
/// 整数属性编辑器，支持最小值/最大值校验。
/// </summary>
public sealed class IntegerPropertyEditorViewModel : PropertyEditorViewModel
{
    private int _value;

    public IntegerPropertyEditorViewModel(StepPropertyDescriptor descriptor, string initialValue)
        : base(descriptor, initialValue)
    {
        int.TryParse(initialValue, out _value);
    }

    public int Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
                OnPropertyChanged(nameof(StringValue));
        }
    }

    public int MinValue => (int)(Descriptor.MinValue ?? int.MinValue);
    public int MaxValue => (int)(Descriptor.MaxValue ?? int.MaxValue);

    public override string StringValue
    {
        get => _value.ToString();
        set
        {
            if (int.TryParse(value, out var parsed))
                Value = parsed;
        }
    }
}
