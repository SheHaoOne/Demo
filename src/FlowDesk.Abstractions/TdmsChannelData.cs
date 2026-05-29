namespace FlowDesk.Abstractions;

/// <summary>
/// TDMS 通道数据，包含通道名称和采样值。
/// </summary>
public sealed class TdmsChannelData
{
    public TdmsChannelData(string groupName, string channelName, double[] values)
    {
        GroupName = groupName;
        ChannelName = channelName;
        Values = values;
    }

    public string GroupName { get; }
    public string ChannelName { get; }
    public double[] Values { get; }

    /// <summary>显示名称（组/通道）。</summary>
    public string DisplayName => $"{GroupName}/{ChannelName}";
}
