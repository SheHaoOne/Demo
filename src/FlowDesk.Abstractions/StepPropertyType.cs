namespace FlowDesk.Abstractions;

/// <summary>
/// 步骤属性值类型。
/// </summary>
public enum StepPropertyType
{
    /// <summary>字符串。</summary>
    String,
    /// <summary>整数。</summary>
    Integer,
    /// <summary>浮点数。</summary>
    Double,
    /// <summary>布尔值。</summary>
    Boolean,
    /// <summary>枚举（有限选项集）。</summary>
    Enum,
    /// <summary>文件路径。</summary>
    FilePath
}
