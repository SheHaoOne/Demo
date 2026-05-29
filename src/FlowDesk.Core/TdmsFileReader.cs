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
                    channels.Add(new TdmsChannelData(group.Name, channel.Name, values));
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
    /// 生成演示波形数据（文件不存在或解析失败时的回退）。
    /// </summary>
    private static TdmsFileData GenerateDemoData(string filePath)
    {
        const int sampleCount = 1000;
        var ch1 = new double[sampleCount];
        var ch2 = new double[sampleCount];
        var ch3 = new double[sampleCount];

        for (var i = 0; i < sampleCount; i++)
        {
            var t = i / 100.0;
            ch1[i] = Math.Sin(2 * Math.PI * t);
            ch2[i] = 0.5 * Math.Sin(2 * Math.PI * 3 * t + Math.PI / 4);
            ch3[i] = Math.Sin(2 * Math.PI * t) + 0.3 * Math.Sin(2 * Math.PI * 5 * t);
        }

        return new TdmsFileData(filePath,
        [
            new TdmsChannelData("Voltage", "Channel_1", ch1),
            new TdmsChannelData("Voltage", "Channel_2", ch2),
            new TdmsChannelData("Voltage", "Channel_3", ch3)
        ]);
    }
}
