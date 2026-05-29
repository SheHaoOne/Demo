using System.Collections.ObjectModel;
using FlowDesk.App.ViewModels.Shared;
using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels.Config;

/// <summary>
/// 配置页面 ViewModel：系统配置、PLC 配置、采集卡配置、传感器配置。
/// </summary>
public sealed class ConfigViewModel : ObservableObject
{
    private int _selectedTabIndex;

    public ConfigViewModel()
    {
        SystemConfig = CreateSection("系统", new Dictionary<string, string>
        {
            ["应用名称"] = "FlowDesk",
            ["语言"] = "zh-CN",
            ["日志级别"] = "Info",
            ["自动保存间隔(秒)"] = "60"
        });

        PlcConfig = CreateSection("PLC", new Dictionary<string, string>
        {
            ["IP 地址"] = "192.168.1.100",
            ["端口"] = "502",
            ["协议类型"] = "Modbus TCP",
            ["超时时间(ms)"] = "3000",
            ["轮询间隔(ms)"] = "100"
        });

        DaqConfig = CreateSection("采集卡", new Dictionary<string, string>
        {
            ["设备名称"] = "Dev1",
            ["采样率(Hz)"] = "1000",
            ["通道数"] = "8",
            ["量程(V)"] = "10",
            ["触发模式"] = "连续采集"
        });

        SensorConfig = CreateSection("传感器", new Dictionary<string, string>
        {
            ["传感器类型"] = "温度传感器",
            ["通信接口"] = "RS485",
            ["波特率"] = "9600",
            ["数据位"] = "8",
            ["校验方式"] = "None"
        });

        SaveCommand = new RelayCommand(_ => Save());
    }

    public ObservableCollection<KeyValueViewModel> SystemConfig { get; }
    public ObservableCollection<KeyValueViewModel> PlcConfig { get; }
    public ObservableCollection<KeyValueViewModel> DaqConfig { get; }
    public ObservableCollection<KeyValueViewModel> SensorConfig { get; }
    public RelayCommand SaveCommand { get; }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => SetProperty(ref _selectedTabIndex, value);
    }

    private static ObservableCollection<KeyValueViewModel> CreateSection(
        string category, Dictionary<string, string> items)
    {
        return new ObservableCollection<KeyValueViewModel>(
            items.Select(kvp => new KeyValueViewModel(kvp.Key, kvp.Value, category)));
    }

    private void Save()
    {
        // 预留：持久化配置到文件
    }
}
