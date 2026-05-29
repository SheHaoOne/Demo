namespace FlowDesk.Abstractions;

/// <summary>
/// 步骤属性描述符，定义属性的元数据（名称、类型、默认值、校验范围等）。
/// </summary>
public sealed class StepPropertyDescriptor
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public string Description { get; init; } = "";
    public string Category { get; init; } = "常规";
    public StepPropertyType PropertyType { get; init; } = StepPropertyType.String;
    public object? DefaultValue { get; init; }
    public string[]? EnumOptions { get; init; }
    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }
    public bool IsRequired { get; init; } = true;
}
