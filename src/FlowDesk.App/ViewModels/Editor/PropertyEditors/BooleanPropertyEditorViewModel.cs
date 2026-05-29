using FlowDesk.Abstractions;

namespace FlowDesk.App.ViewModels.Editor.PropertyEditors;

/// <summary>
/// 布尔属性编辑器。
/// </summary>
public sealed class BooleanPropertyEditorViewModel : PropertyEditorViewModel
{
    private bool _value;

    public BooleanPropertyEditorViewModel(StepPropertyDescriptor descriptor, string initialValue)
        : base(descriptor, initialValue)
    {
        bool.TryParse(initialValue, out _value);
    }

    public bool Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
                OnPropertyChanged(nameof(StringValue));
        }
    }

    public override string StringValue
    {
        get => _value.ToString();
        set
        {
            if (bool.TryParse(value, out var parsed))
                Value = parsed;
        }
    }
}
