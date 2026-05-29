using FlowDesk.Abstractions;

namespace FlowDesk.Core;

/// <summary>
/// TDMS 文件读取契约。
/// </summary>
public interface ITdmsReader
{
    TdmsFileData Read(string filePath);
}
