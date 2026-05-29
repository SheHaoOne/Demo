using System.Collections.ObjectModel;
using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels;

/// <summary>
/// 配置页面 ViewModel：系统配置、PLC 配置、采集卡配置、传感器配置。
/// </summary>
public sealed class ConfigViewModel : ObservableObject
{
    private int _selectedTabIndex;

    public ConfigViewModel()
    {
        SystemConfig = CreateDefaultConfig("系统", new Dictionary<string, string>
        {
            ["应用名称"] = "FlowDesk",
            ["语言"] = "zh-CN",
            ["日志级别"] = "Info",
            ["自动保存间隔(秒)"] = "60"
        });

        PlcConfig = CreateDefaultConfig("PLC", new Dictionary<string, string>
        {
            ["IP 地址"] = "192.168.1.100",
            ["端口"] = "502",
            ["协议类型"] = "Modbus TCP",
            ["超时时间(ms)"] = "3000",
            ["轮询间隔(ms)"] = "100"
        });

        DaqConfig = CreateDefaultConfig("采集卡", new Dictionary<string, string>
        {
            ["设备名称"] = "Dev1",
            ["采样率(Hz)"] = "1000",
            ["通道数"] = "8",
            ["量程(V)"] = "10",
            ["触发模式"] = "连续采集"
        });

        SensorConfig = CreateDefaultConfig("传感器", new Dictionary<string, string>
        {
            ["传感器类型"] = "温度传感器",
            ["通信接口"] = "RS485",
            ["波特率"] = "9600",
            ["数据位"] = "8",
            ["校验方式"] = "None"
        });

        SaveCommand = new RelayCommand(_ => Save());
    }

    public ObservableCollection<ConfigItemViewModel> SystemConfig { get; }
    public ObservableCollection<ConfigItemViewModel> PlcConfig { get; }
    public ObservableCollection<ConfigItemViewModel> DaqConfig { get; }
    public ObservableCollection<ConfigItemViewModel> SensorConfig { get; }

    public RelayCommand SaveCommand { get; }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => SetProperty(ref _selectedTabIndex, value);
    }

    private static ObservableCollection<ConfigItemViewModel> CreateDefaultConfig(
        string category, Dictionary<string, string> items)
    {
        var collection = new ObservableCollection<ConfigItemViewModel>();
        foreach (var kvp in items)
        {
            collection.Add(new ConfigItemViewModel(category, kvp.Key, kvp.Value));
        }
        return collection;
    }

    private void Save()
    {
        // 预留：持久化配置到文件
    }
}

/// <summary>
/// 单条配置项。
/// </summary>
public sealed class ConfigItemViewModel : ObservableObject
{
    private string _value;

    public ConfigItemViewModel(string category, string key, string value)
    {
        Category = category;
        Key = key;
        _value = value;
    }

    public string Category { get; }
    public string Key { get; }

    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}
