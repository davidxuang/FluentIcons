using CommunityToolkit.Mvvm.Input;
using FluentIcons.Gallery.Helpers;
using FluentIcons.Gallery.Models;

namespace FluentIcons.Gallery.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    public static IReadOnlyList<IconInfo> Icons { get; } = Enum.GetValues<Icon>()
        .Select(icon => new IconInfo { Name = icon.ToString(), Value = icon })
        .ToImmutableArray();

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;
    [ObservableProperty]
    public partial IReadOnlyList<IconInfo> FilteredIcons { get; set; } = Icons;
    
    [ObservableProperty]
    public partial bool UsesSymbol { get; set; } = false;
    [ObservableProperty]
    public partial IconVariant IconVariant { get; set; } = IconVariant.Regular;

    [ObservableProperty]
    public partial IconInfo Selected { get; set; } = Icons[0].Let(x => x.IsSelected = true);

    [RelayCommand]
    public void SelectIcon(IconInfo icon)
    {
        Selected?.IsSelected = false;
        icon.IsSelected = true;
        Selected = icon;
    }

    async partial void OnSearchTextChanged(string value)
    {
        await Task.Yield();
        if (string.IsNullOrEmpty(value))
        {
            FilteredIcons = Icons;
        }
        else
        {
            FilteredIcons = Icons
                .Where(x => x.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
                .ToImmutableArray();
        }
    }
}
