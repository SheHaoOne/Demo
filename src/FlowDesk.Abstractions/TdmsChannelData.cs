namespace FlowDesk.Abstractions;

/// <summary>
/// TDMS 通道数据，包含通道名称、采样率和采样值。
/// </summary>
public sealed class TdmsChannelData
{
    public TdmsChannelData(string groupName, string channelName, double[] values, double sampleRate = 0)
    {
        GroupName = groupName;
        ChannelName = channelName;
        Values = values;
        SampleRate = sampleRate;
    }

    public string GroupName { get; }
    public string ChannelName { get; }
    public double[] Values { get; }

    /// <summary>采样率（Hz）。0 表示未知。</summary>
    public double SampleRate { get; }

    /// <summary>采样间隔（秒）。采样率未知时返回 0。</summary>
    public double SampleInterval => SampleRate > 0 ? 1.0 / SampleRate : 0;

    /// <summary>数据总时长（秒）。采样率未知时返回 0。</summary>
    public double Duration => SampleRate > 0 ? Values.Length / SampleRate : 0;

    /// <summary>显示名称（组/通道）。</summary>
    public string DisplayName => $"{GroupName}/{ChannelName}";
}
