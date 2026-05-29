namespace FlowDesk.Abstractions;

/// <summary>
/// TDMS 文件解析结果。
/// </summary>
public sealed class TdmsFileData
{
    public TdmsFileData(string filePath, IReadOnlyList<TdmsChannelData> channels)
    {
        FilePath = filePath;
        Channels = channels;
    }

    public string FilePath { get; }
    public IReadOnlyList<TdmsChannelData> Channels { get; }
}
