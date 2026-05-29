using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FlowDesk.Abstractions;
using ScottPlot;

namespace FlowDesk.App.Views.Home;

/// <summary>
/// TDMS 波形图查看窗口，使用 ScottPlot 库绘制，支持缩放、平移和多通道切换。
/// </summary>
public partial class WaveformChartWindow : Window
{
    private static readonly ScottPlot.Color[] PlotColors =
    [
        ScottPlot.Color.FromHex("#2196F3"),
        ScottPlot.Color.FromHex("#4CAF50"),
        ScottPlot.Color.FromHex("#F44336"),
        ScottPlot.Color.FromHex("#FF9800"),
        ScottPlot.Color.FromHex("#9C27B0"),
        ScottPlot.Color.FromHex("#00BCD4"),
        ScottPlot.Color.FromHex("#FFC107"),
        ScottPlot.Color.FromHex("#795548")
    ];

    private readonly TdmsFileData _data;
    private readonly Dictionary<int, bool> _channelVisible = new();

    public WaveformChartWindow(TdmsFileData data)
    {
        _data = data;
        InitializeComponent();

        FilePathText.Text = $"文件：{data.FilePath}  |  {data.Channels.Count} 个通道";

        for (var i = 0; i < data.Channels.Count; i++)
        {
            _channelVisible[i] = true;
            var wpfColor = System.Windows.Media.Color.FromRgb(
                PlotColors[i % PlotColors.Length].R,
                PlotColors[i % PlotColors.Length].G,
                PlotColors[i % PlotColors.Length].B);

            var cb = new CheckBox
            {
                Content = data.Channels[i].DisplayName,
                IsChecked = true,
                Foreground = new SolidColorBrush(wpfColor),
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 16, 0),
                Tag = i
            };
            cb.Checked += ChannelToggle_Changed;
            cb.Unchecked += ChannelToggle_Changed;
            ChannelList.Items.Add(cb);
        }

        DrawChart();
    }

    private void ChannelToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox { Tag: int idx })
        {
            _channelVisible[idx] = ((CheckBox)sender).IsChecked == true;
            DrawChart();
        }
    }

    private void DrawChart()
    {
        var plt = WpfPlot.Plot;
        plt.Clear();

        plt.Title("TDMS 波形数据");
        plt.XLabel("采样点");
        plt.YLabel("幅值");

        for (var i = 0; i < _data.Channels.Count; i++)
        {
            if (!_channelVisible.GetValueOrDefault(i, false)) continue;

            var ch = _data.Channels[i];
            if (ch.Values.Length == 0) continue;

            var sig = plt.Add.Signal(ch.Values);
            sig.LegendText = ch.DisplayName;
            sig.Color = PlotColors[i % PlotColors.Length];
            sig.LineWidth = 1.5f;
        }

        plt.Legend.IsVisible = true;
        WpfPlot.Refresh();
    }
}
