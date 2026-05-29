using FlowDesk.Abstractions;
using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels.Editor.PropertyEditors;

/// <summary>
/// 属性编辑器基类（OCP：新增属性类型只需添加子类和对应 DataTemplate）。
/// </summary>
public abstract class PropertyEditorViewModel : ObservableObject
{
    protected PropertyEditorViewModel(StepPropertyDescriptor descriptor, string initialValue)
    {
        Descriptor = descriptor;
    }

    public StepPropertyDescriptor Descriptor { get; }
    public string Name => Descriptor.Name;
    public string DisplayName => Descriptor.DisplayName;
    public string Description => Descriptor.Description;
    public string Category => Descriptor.Category;
    public bool IsRequired => Descriptor.IsRequired;

    /// <summary>
    /// 获取或设置序列化后的字符串值（与 Settings 字典同步）。
    /// </summary>
    public abstract string StringValue { get; set; }

    /// <summary>
    /// 工厂方法：根据属性描述符创建对应的编辑器实例。
    /// </summary>
    public static PropertyEditorViewModel Create(StepPropertyDescriptor descriptor, string currentValue)
    {
        return descriptor.PropertyType switch
        {
            StepPropertyType.Integer => new IntegerPropertyEditorViewModel(descriptor, currentValue),
            StepPropertyType.Double => new DoublePropertyEditorViewModel(descriptor, currentValue),
            StepPropertyType.Boolean => new BooleanPropertyEditorViewModel(descriptor, currentValue),
            StepPropertyType.Enum => new EnumPropertyEditorViewModel(descriptor, currentValue),
            StepPropertyType.FilePath => new FilePathPropertyEditorViewModel(descriptor, currentValue),
            _ => new StringPropertyEditorViewModel(descriptor, currentValue)
        };
    }
}
