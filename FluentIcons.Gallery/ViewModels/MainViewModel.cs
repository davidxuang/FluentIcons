using Avalonia.Collections;
using Avalonia.Input.Platform;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Gallery.Models;

namespace FluentIcons.Gallery.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    public static readonly IReadOnlyList<IconInfo> Icons = Enum.GetValues<Icon>()
        .Select(icon => new IconInfo(icon))
        .OrderBy(x => x.Name, StringComparer.Ordinal)
        .ToImmutableArray();
    [ObservableProperty]
    public partial DataGridCollectionView IconsView { get; set; } = new DataGridCollectionView(Icons);

    public MainViewModel()
    {
        IconsView.Filter = (item) => item is IconInfo info
            && (!IsFilterEnabled || info.Value.IsAvailable(UsesSymbol ? IconSize.Resizable : IconSize, IconVariant))
            && (string.IsNullOrEmpty(SearchText) || _searchTerms.All(term => info.Name.Contains(term, StringComparison.OrdinalIgnoreCase)))
            && (!UsesSymbol || info.IsSymbolAvailable);
    }

    private void RefreshIconsView()
    {
        IconsView.Refresh();
        if (!IconsView.Contains(Selected) && IconsView.Count > 0 && IconsView.GetItemAt(0) is IconInfo info)
        {
            Selected = info;
        }
    }

    [ObservableProperty]
    public partial bool IsFilterEnabled { get; set; } = true;
    partial void OnIsFilterEnabledChanged(bool value) => RefreshIconsView();

    private IEnumerable<string> _searchTerms = Array.Empty<string>();
    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;
    partial void OnSearchTextChanged(string value)
    {
        _searchTerms = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        IconsView.Refresh();
    }

    [ObservableProperty]
    public partial FlowDirection FlowDirection { get; set; } = FlowDirection.LeftToRight;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Xaml), nameof(XamlExtension), nameof(CSharp), nameof(CSharpMarkup))]
    public partial bool UsesSymbol { get; set; } = true;
    partial void OnUsesSymbolChanged(bool value) => RefreshIconsView();

    [ObservableProperty]
    public partial IconSize IconSize { get; set; } = IconSize.Resizable;
    partial void OnIconSizeChanged(IconSize value) => RefreshIconsView();
    [RelayCommand]
    public void SetIconSize(IconSize size) => IconSize = size;
    public IEnumerable<IconSize> IconSizes => Enum.GetValues<IconSize>().Where(x => x != IconSize.Resizable);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Xaml), nameof(XamlExtension), nameof(CSharp), nameof(CSharpMarkup))]
    public partial IconVariant IconVariant { get; set; } = IconVariant.Regular;
    partial void OnIconVariantChanged(IconVariant value) => RefreshIconsView();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Xaml), nameof(XamlExtension), nameof(CSharp), nameof(CSharpMarkup))]
    public partial IconInfo Selected { get; set; } = Icons.First(x => x.IsSymbolAvailable);
    [RelayCommand]
    public void SetIcon(IconInfo icon) => Selected = icon;

    private string Prefix => UsesSymbol ? "Symbol" : "Fluent";
    private string Property => UsesSymbol ? "Symbol" : "Icon";
    public string Xaml => IconVariant == IconVariant.Regular
        ? $"<ic:{Prefix}Icon {Property}=\"{Selected?.Name}\" />"
        : $"<ic:{Prefix}Icon {Property}=\"{Selected?.Name}\" IconVariant=\"{IconVariant}\" />";
    public string XamlExtension => IconVariant == IconVariant.Regular
        ? $"{{icx:{Prefix}Icon {Property}={Selected?.Name}}}"
        : $"{{icx:{Prefix}Icon {Property}={Selected?.Name}, IconVariant={IconVariant}}}";
    public string CSharp => IconVariant == IconVariant.Regular
        ? $"new {Prefix}Icon {{ {Property} = {Property}.{Selected?.Name} }};"
        : $"new {Prefix}Icon {{ {Property} = {Property}.{Selected?.Name}, IconVariant = IconVariant.{IconVariant} }};";
    public string CSharpMarkup => IconVariant == IconVariant.Regular
        ? $"new {Prefix}Icon().{Property}({Property}.{Selected?.Name})"
        : $"new {Prefix}Icon().{Property}({Property}.{Selected?.Name}).IconVariant(IconVariant.{IconVariant})";

    [RelayCommand]
    public void CopyXaml() => App.Clipboard?.SetTextAsync(Xaml);
    [RelayCommand]
    public void CopyXamlExtension() => App.Clipboard?.SetTextAsync(XamlExtension);
    [RelayCommand]
    public void CopyCSharp() => App.Clipboard?.SetTextAsync(CSharp);
    [RelayCommand]
    public void CopyCSharpMarkup() => App.Clipboard?.SetTextAsync(CSharpMarkup);

    public string Version => typeof(Icon).Assembly.GetName().Version?.ToString(3) ?? "–";
}
