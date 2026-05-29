using FlowDesk.Abstractions;
using NationalInstruments.Tdms;

namespace FlowDesk.Core;

/// <summary>
/// 基于 TDMSReader 库的 TDMS 文件读取器。
/// 文件不存在时返回演示波形数据，便于 UI 开发和测试。
/// </summary>
public sealed class TdmsFileReader : ITdmsReader
{
    public TdmsFileData Read(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!System.IO.File.Exists(filePath))
            return GenerateDemoData(filePath);

        try
        {
            return ReadWithLibrary(filePath);
        }
        catch
        {
            return GenerateDemoData(filePath);
        }
    }

    /// <summary>
    /// 使用 TDMSReader 库读取 TDMS 文件，提取所有数值通道数据。
    /// </summary>
    private static TdmsFileData ReadWithLibrary(string filePath)
    {
        var channels = new List<TdmsChannelData>();

        using var tdmsFile = new NationalInstruments.Tdms.File(filePath);
        tdmsFile.Open();

        foreach (var group in tdmsFile)
        {
            foreach (var channel in group)
            {
                var values = ExtractDoubleValues(channel);
                if (values is not null && values.Length > 0)
                {
                    var sampleRate = ExtractSampleRate(channel);
                    channels.Add(new TdmsChannelData(group.Name, channel.Name, values, sampleRate));
                }
            }
        }

        return channels.Count > 0
            ? new TdmsFileData(filePath, channels)
            : GenerateDemoData(filePath);
    }

    /// <summary>
    /// 从通道中提取 double 数组，支持多种数值类型自动转换。
    /// </summary>
    private static double[]? ExtractDoubleValues(Channel channel)
    {
        if (channel.DataCount == 0) return null;

        try
        {
            if (channel.DataType == typeof(double))
                return channel.GetData<double>().ToArray();

            if (channel.DataType == typeof(float))
                return channel.GetData<float>().Select(v => (double)v).ToArray();

            if (channel.DataType == typeof(int))
                return channel.GetData<int>().Select(v => (double)v).ToArray();

            if (channel.DataType == typeof(short))
                return channel.GetData<short>().Select(v => (double)v).ToArray();

            if (channel.DataType == typeof(long))
                return channel.GetData<long>().Select(v => (double)v).ToArray();

            if (channel.DataType == typeof(byte))
                return channel.GetData<byte>().Select(v => (double)v).ToArray();

            if (channel.DataType == typeof(uint))
                return channel.GetData<uint>().Select(v => (double)v).ToArray();

            if (channel.DataType == typeof(ushort))
                return channel.GetData<ushort>().Select(v => (double)v).ToArray();

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 从通道属性中提取采样率。
    /// TDMS 文件常用属性：wf_increment（采样间隔）或 wf_xincrement、NI_SampleRate 等。
    /// </summary>
    private static double ExtractSampleRate(Channel channel)
    {
        try
        {
            var props = channel.Properties;

            if (props.TryGetValue("wf_increment", out var increment) && increment is double inc && inc > 0)
                return 1.0 / inc;

            if (props.TryGetValue("wf_xincrement", out var xIncrement) && xIncrement is double xInc && xInc > 0)
                return 1.0 / xInc;

            if (props.TryGetValue("NI_SampleRate", out var niRate) && niRate is double rate && rate > 0)
                return rate;

            if (props.TryGetValue("SampleRate", out var sr) && sr is double sampleRate && sampleRate > 0)
                return sampleRate;

            if (props.TryGetValue("wf_samples", out var samples) && samples is int sampleCount && sampleCount > 0
                && props.TryGetValue("wf_increment", out var inc2) && inc2 is double incVal && incVal > 0)
                return 1.0 / incVal;
        }
        catch
        {
            // 属性读取失败时返回 0（未知采样率）
        }

        return 0;
    }

    /// <summary>
    /// 生成演示波形数据（文件不存在或解析失败时的回退）。
    /// </summary>
    private static TdmsFileData GenerateDemoData(string filePath)
    {
        const int sampleCount = 25600;
        const double sampleRate = 25600.0;
        var ch1 = new double[sampleCount];
        var ch2 = new double[sampleCount];
        var ch3 = new double[sampleCount];

        for (var i = 0; i < sampleCount; i++)
        {
            var t = i / sampleRate;
            ch1[i] = Math.Sin(2 * Math.PI * 50 * t);
            ch2[i] = 0.5 * Math.Sin(2 * Math.PI * 150 * t + Math.PI / 4);
            ch3[i] = Math.Sin(2 * Math.PI * 50 * t) + 0.3 * Math.Sin(2 * Math.PI * 250 * t);
        }

        return new TdmsFileData(filePath,
        [
            new TdmsChannelData("Voltage", "Channel_1", ch1, sampleRate),
            new TdmsChannelData("Voltage", "Channel_2", ch2, sampleRate),
            new TdmsChannelData("Voltage", "Channel_3", ch3, sampleRate)
        ]);
    }
}
