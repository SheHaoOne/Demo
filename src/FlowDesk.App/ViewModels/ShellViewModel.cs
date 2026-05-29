using System.Collections.ObjectModel;
using FlowDesk.UI;
using FlowDesk.UI.Mvvm;
using FlowDesk.UI.Navigation;

namespace FlowDesk.App.ViewModels;

/// <summary>
/// 应用主框架 ViewModel，管理导航菜单、内容区切换和全局设置。
/// </summary>
public sealed class ShellViewModel : ObservableObject
{
    private NavigationItem? _selectedNavItem;
    private ObservableObject? _currentPage;
    private NavMenuPosition _menuPosition = NavMenuPosition.Left;
    private string _themeLabel = "切换深色";

    private readonly HomeViewModel _homePage;
    private readonly ConfigViewModel _configPage;
    private readonly SequenceEditorViewModel _editorPage;

    public ShellViewModel(HomeViewModel homePage, ConfigViewModel configPage, SequenceEditorViewModel editorPage)
    {
        _homePage = homePage;
        _configPage = configPage;
        _editorPage = editorPage;

        NavItems =
        [
            new NavigationItem("home", "首页", "\uE80F"),
            new NavigationItem("config", "配置", "\uE713"),
            new NavigationItem("editor", "编辑器", "\uE70F")
        ];

        ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
        SetMenuPositionCommand = new RelayCommand(param =>
        {
            if (param is string pos && Enum.TryParse<NavMenuPosition>(pos, true, out var position))
                MenuPosition = position;
        });

        SelectedNavItem = NavItems[0];
    }

    public ObservableCollection<NavigationItem> NavItems { get; }

    public RelayCommand ToggleThemeCommand { get; }

    public RelayCommand SetMenuPositionCommand { get; }

    public NavigationItem? SelectedNavItem
    {
        get => _selectedNavItem;
        set
        {
            if (_selectedNavItem is not null) _selectedNavItem.IsSelected = false;
            if (SetProperty(ref _selectedNavItem, value))
            {
                if (value is not null) value.IsSelected = true;
                NavigateTo(value?.Key);
            }
        }
    }

    public ObservableObject? CurrentPage
    {
        get => _currentPage;
        private set => SetProperty(ref _currentPage, value);
    }

    public NavMenuPosition MenuPosition
    {
        get => _menuPosition;
        set => SetProperty(ref _menuPosition, value);
    }

    public string ThemeLabel
    {
        get => _themeLabel;
        private set => SetProperty(ref _themeLabel, value);
    }

    private void NavigateTo(string? key)
    {
        CurrentPage = key switch
        {
            "home" => _homePage,
            "config" => _configPage,
            "editor" => _editorPage,
            _ => null
        };
    }

    private void ToggleTheme()
    {
        ThemeManager.ToggleTheme();
        ThemeLabel = ThemeManager.CurrentTheme == AppTheme.Light ? "切换深色" : "切换亮色";
    }
}
