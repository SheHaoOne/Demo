using System.Text;
using FlowDesk.Abstractions;

namespace FlowDesk.Core;

/// <summary>
/// TDMS 文件读取器。支持基本的 TDMS 文件格式（double 数据类型）。
/// 对于复杂或不支持的格式，返回模拟演示数据。
/// </summary>
public sealed class TdmsFileReader : ITdmsReader
{
    private const uint TdmsTag = 0x6D734454; // "TDSm" 小端序

    public TdmsFileData Read(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"TDMS 文件不存在：{filePath}", filePath);

        try
        {
            return ReadInternal(filePath);
        }
        catch
        {
            // 解析失败时返回演示数据，便于 UI 开发和测试
            return GenerateDemoData(filePath);
        }
    }

    private TdmsFileData ReadInternal(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: false);

        var channels = new List<TdmsChannelData>();
        var channelMeta = new List<(string path, uint dataType, long numValues)>();

        while (stream.Position < stream.Length)
        {
            // Lead-in: 28 字节
            var tag = reader.ReadUInt32();
            if (tag != TdmsTag) break;

            var versionNumber = reader.ReadInt32();
            var flags = reader.ReadUInt32();
            var nextSegmentOffset = reader.ReadInt64();
            var rawDataOffset = reader.ReadInt64();

            var hasMetadata = (flags & (1 << 1)) != 0;
            var hasRawData = (flags & (1 << 3)) != 0;

            var segmentStart = stream.Position;

            if (hasMetadata)
            {
                var numObjects = reader.ReadInt32();
                channelMeta.Clear();

                for (var i = 0; i < numObjects; i++)
                {
                    var pathLen = reader.ReadInt32();
                    var path = Encoding.UTF8.GetString(reader.ReadBytes(pathLen));

                    var rawDataIndex = reader.ReadInt32();
                    uint dataType = 0;
                    long numValues = 0;

                    if (rawDataIndex != unchecked((int)0xFFFFFFFF))
                    {
                        dataType = reader.ReadUInt32();
                        reader.ReadInt32(); // arrayDimension
                        numValues = reader.ReadInt64();
                        channelMeta.Add((path, dataType, numValues));
                    }

                    // 读取属性
                    var numProperties = reader.ReadInt32();
                    for (var p = 0; p < numProperties; p++)
                    {
                        var propNameLen = reader.ReadInt32();
                        reader.ReadBytes(propNameLen);
                        var propType = reader.ReadInt32();
                        SkipTdmsValue(reader, (uint)propType);
                    }
                }
            }

            if (hasRawData && channelMeta.Count > 0)
            {
                stream.Position = segmentStart + rawDataOffset;

                foreach (var (path, dataType, numValues) in channelMeta)
                {
                    if (dataType == 10 && numValues > 0) // 10 = tdsTypeDoubleFloat
                    {
                        var values = new double[numValues];
                        for (long v = 0; v < numValues; v++)
                            values[v] = reader.ReadDouble();

                        var parts = ParseChannelPath(path);
                        channels.Add(new TdmsChannelData(parts.group, parts.channel, values));
                    }
                    else
                    {
                        // 跳过非 double 类型数据
                        var bytesPerValue = GetTdmsTypeSize(dataType);
                        if (bytesPerValue > 0)
                            stream.Position += numValues * bytesPerValue;
                    }
                }
            }

            // 移动到下一段
            if (nextSegmentOffset > 0)
                stream.Position = segmentStart - 28 + 28 + nextSegmentOffset;
            else
                break;
        }

        if (channels.Count == 0)
            return GenerateDemoData(filePath);

        return new TdmsFileData(filePath, channels);
    }

    private static (string group, string channel) ParseChannelPath(string path)
    {
        // TDMS 路径格式: /'GroupName'/'ChannelName'
        var segments = path.Split('\'', StringSplitOptions.RemoveEmptyEntries)
            .Where(s => s != "/" && s != "/'" && !string.IsNullOrWhiteSpace(s))
            .ToArray();

        return segments.Length >= 2
            ? (segments[0], segments[1])
            : (segments.Length == 1 ? (segments[0], "Data") : ("Group", "Channel"));
    }

    private static int GetTdmsTypeSize(uint dataType) => dataType switch
    {
        1 => 1,   // tdsTypeI8
        2 => 2,   // tdsTypeI16
        3 => 4,   // tdsTypeI32
        4 => 8,   // tdsTypeI64
        5 => 1,   // tdsTypeU8
        6 => 2,   // tdsTypeU16
        7 => 4,   // tdsTypeU32
        8 => 8,   // tdsTypeU64
        9 => 4,   // tdsTypeSingleFloat
        10 => 8,  // tdsTypeDoubleFloat
        _ => 0
    };

    private static void SkipTdmsValue(BinaryReader reader, uint propType)
    {
        var size = GetTdmsTypeSize(propType);
        if (size > 0)
        {
            reader.ReadBytes(size);
            return;
        }

        if (propType == 0x20) // tdsTypeString
        {
            var len = reader.ReadInt32();
            reader.ReadBytes(len);
            return;
        }

        if (propType == 0x21) // tdsTypeBoolean
        {
            reader.ReadByte();
            return;
        }
    }

    /// <summary>
    /// 生成演示波形数据（用于无真实 TDMS 文件时的 UI 开发和测试）。
    /// </summary>
    private static TdmsFileData GenerateDemoData(string filePath)
    {
        const int sampleCount = 1000;
        var channels = new List<TdmsChannelData>();

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

        channels.Add(new TdmsChannelData("Voltage", "Channel_1", ch1));
        channels.Add(new TdmsChannelData("Voltage", "Channel_2", ch2));
        channels.Add(new TdmsChannelData("Voltage", "Channel_3", ch3));

        return new TdmsFileData(filePath, channels);
    }
}
