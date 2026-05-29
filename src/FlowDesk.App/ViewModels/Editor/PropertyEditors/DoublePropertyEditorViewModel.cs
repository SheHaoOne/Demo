using System.Globalization;
using FlowDesk.Abstractions;

namespace FlowDesk.App.ViewModels.Editor.PropertyEditors;

/// <summary>
/// 浮点数属性编辑器。
/// </summary>
public sealed class DoublePropertyEditorViewModel : PropertyEditorViewModel
{
    private double _value;

    public DoublePropertyEditorViewModel(StepPropertyDescriptor descriptor, string initialValue)
        : base(descriptor, initialValue)
    {
        double.TryParse(initialValue, CultureInfo.InvariantCulture, out _value);
    }

    public double Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
                OnPropertyChanged(nameof(StringValue));
        }
    }

    public double MinValue => Descriptor.MinValue ?? double.MinValue;
    public double MaxValue => Descriptor.MaxValue ?? double.MaxValue;

    public override string StringValue
    {
        get => _value.ToString(CultureInfo.InvariantCulture);
        set
        {
            if (double.TryParse(value, CultureInfo.InvariantCulture, out var parsed))
                Value = parsed;
        }
    }
}
