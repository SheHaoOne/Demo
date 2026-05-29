using FlowDesk.UI.Mvvm;

namespace FlowDesk.App.ViewModels.Shared;

/// <summary>
/// 通用键值对 ViewModel，用于设置编辑和配置项。
/// 消除原 SettingViewModel 和 ConfigItemViewModel 的代码重复。
/// </summary>
public sealed class KeyValueViewModel : ObservableObject
{
    private string _value;

    public KeyValueViewModel(string key, string value, string category = "")
    {
        Key = key;
        _value = value;
        Category = category;
    }

    public string Key { get; }

    public string Category { get; }

    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}
