using System.Collections.ObjectModel;
using FlowDesk.UI.Mvvm;
using FlowDesk.UI.Navigation;
using FlowDesk.UI.Services;
using FlowDesk.UI.Themes;

namespace FlowDesk.App.ViewModels.Shell;

/// <summary>
/// 应用主框架 ViewModel（SRP：只负责导航切换和布局设置）。
/// 主题切换委托给 IThemeService。
/// </summary>
public sealed class ShellViewModel : ObservableObject
{
    private readonly IThemeService _themeService;
    private readonly Dictionary<string, ObservableObject> _pageRegistry;
    private NavigationItem? _selectedNavItem;
    private ObservableObject? _currentPage;
    private NavMenuPosition _menuPosition = NavMenuPosition.Left;
    private string _themeLabel = "切换深色";

    public ShellViewModel(IThemeService themeService, IReadOnlyList<(string key, string label, string icon, ObservableObject page)> pages)
    {
        _themeService = themeService;
        _pageRegistry = new Dictionary<string, ObservableObject>(StringComparer.OrdinalIgnoreCase);

        NavItems = [];
        foreach (var (key, label, icon, page) in pages)
        {
            NavItems.Add(new NavigationItem(key, label, icon));
            _pageRegistry[key] = page;
        }

        ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
        SetMenuPositionCommand = new RelayCommand(param =>
        {
            if (param is string pos && Enum.TryParse<NavMenuPosition>(pos, true, out var position))
                MenuPosition = position;
        });

        if (NavItems.Count > 0)
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
                CurrentPage = value is not null && _pageRegistry.TryGetValue(value.Key, out var page) ? page : null;
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

    private void ToggleTheme()
    {
        _themeService.ToggleTheme();
        ThemeLabel = _themeService.CurrentTheme == AppTheme.Light ? "切换深色" : "切换亮色";
    }
}
