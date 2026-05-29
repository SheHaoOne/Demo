using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlowDesk.Abstractions;

namespace FlowDesk.App.Views.Home;

/// <summary>
/// TDMS 波形图查看窗口，使用纯 WPF Polyline 绘制。
/// </summary>
public partial class WaveformChartWindow : Window
{
    private static readonly Color[] ChannelColors =
    [
        Color.FromRgb(33, 150, 243),   // 蓝
        Color.FromRgb(76, 175, 80),    // 绿
        Color.FromRgb(244, 67, 54),    // 红
        Color.FromRgb(255, 152, 0),    // 橙
        Color.FromRgb(156, 39, 176),   // 紫
        Color.FromRgb(0, 188, 212),    // 青
        Color.FromRgb(255, 193, 7),    // 黄
        Color.FromRgb(121, 85, 72)     // 棕
    ];

    private readonly TdmsFileData _data;
    private readonly Dictionary<int, bool> _channelVisible = new();
    private readonly List<CheckBox> _checkboxes = [];

    public WaveformChartWindow(TdmsFileData data)
    {
        _data = data;
        InitializeComponent();

        FilePathText.Text = $"文件：{data.FilePath}  |  {data.Channels.Count} 个通道";

        for (var i = 0; i < data.Channels.Count; i++)
        {
            _channelVisible[i] = true;
            var color = ChannelColors[i % ChannelColors.Length];
            var cb = new CheckBox
            {
                Content = data.Channels[i].DisplayName,
                IsChecked = true,
                Foreground = new SolidColorBrush(color),
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 16, 0),
                Tag = i
            };
            cb.Checked += ChannelToggle_Changed;
            cb.Unchecked += ChannelToggle_Changed;
            _checkboxes.Add(cb);
            ChannelList.Items.Add(cb);
        }
    }

    private void ChannelToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox { Tag: int idx })
        {
            _channelVisible[idx] = ((CheckBox)sender).IsChecked == true;
            DrawChart();
        }
    }

    private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        DrawChart();
    }

    private void DrawChart()
    {
        ChartCanvas.Children.Clear();
        YAxisCanvas.Children.Clear();
        XAxisCanvas.Children.Clear();

        var width = ChartCanvas.ActualWidth;
        var height = ChartCanvas.ActualHeight;
        if (width <= 0 || height <= 0) return;

        // 计算全局 Y 范围
        var globalMin = double.MaxValue;
        var globalMax = double.MinValue;
        var maxSamples = 0;

        for (var i = 0; i < _data.Channels.Count; i++)
        {
            if (!_channelVisible.GetValueOrDefault(i, false)) continue;
            var ch = _data.Channels[i];
            if (ch.Values.Length == 0) continue;

            foreach (var v in ch.Values)
            {
                if (v < globalMin) globalMin = v;
                if (v > globalMax) globalMax = v;
            }
            if (ch.Values.Length > maxSamples) maxSamples = ch.Values.Length;
        }

        if (maxSamples == 0 || globalMin >= globalMax)
        {
            globalMin = -1;
            globalMax = 1;
        }

        var yMargin = (globalMax - globalMin) * 0.1;
        globalMin -= yMargin;
        globalMax += yMargin;

        // 绘制网格线
        DrawGridLines(width, height, globalMin, globalMax, maxSamples);

        // 绘制波形
        for (var i = 0; i < _data.Channels.Count; i++)
        {
            if (!_channelVisible.GetValueOrDefault(i, false)) continue;
            var ch = _data.Channels[i];
            if (ch.Values.Length == 0) continue;

            var color = ChannelColors[i % ChannelColors.Length];
            DrawWaveform(ch.Values, width, height, globalMin, globalMax, color);
        }
    }

    private void DrawGridLines(double width, double height, double yMin, double yMax, int xMax)
    {
        const int gridCount = 5;

        // 水平网格线 + Y 轴标签
        for (var i = 0; i <= gridCount; i++)
        {
            var y = height * i / gridCount;
            var line = new Line
            {
                X1 = 0, Y1 = y, X2 = width, Y2 = y,
                Stroke = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128)),
                StrokeThickness = 1
            };
            ChartCanvas.Children.Add(line);

            var value = yMax - (yMax - yMin) * i / gridCount;
            var label = new TextBlock
            {
                Text = value.ToString("F2"),
                FontSize = 10,
                Foreground = (Brush)FindResource("SecondaryTextBrush")
            };
            Canvas.SetRight(label, 4);
            Canvas.SetTop(label, y - 7 + 20); // +20 考虑 canvas margin
            YAxisCanvas.Children.Add(label);
        }

        // 垂直网格线 + X 轴标签
        for (var i = 0; i <= gridCount; i++)
        {
            var x = width * i / gridCount;
            var line = new Line
            {
                X1 = x, Y1 = 0, X2 = x, Y2 = height,
                Stroke = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128)),
                StrokeThickness = 1
            };
            ChartCanvas.Children.Add(line);

            var sampleIndex = xMax * i / gridCount;
            var label = new TextBlock
            {
                Text = sampleIndex.ToString(),
                FontSize = 10,
                Foreground = (Brush)FindResource("SecondaryTextBrush")
            };
            Canvas.SetLeft(label, x + 50 - 10); // +50 考虑 left margin
            Canvas.SetTop(label, 4);
            XAxisCanvas.Children.Add(label);
        }
    }

    private void DrawWaveform(double[] values, double width, double height, double yMin, double yMax, Color color)
    {
        var polyline = new Polyline
        {
            Stroke = new SolidColorBrush(color),
            StrokeThickness = 1.5,
            StrokeLineJoin = PenLineJoin.Round
        };

        var yRange = yMax - yMin;
        for (var i = 0; i < values.Length; i++)
        {
            var x = width * i / (values.Length - 1);
            var y = height - height * (values[i] - yMin) / yRange;
            polyline.Points.Add(new Point(x, y));
        }

        ChartCanvas.Children.Add(polyline);
    }
}
