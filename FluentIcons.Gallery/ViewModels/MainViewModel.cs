using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Gallery.Helpers;
using FluentIcons.Gallery.Models;

namespace FluentIcons.Gallery.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    public static IReadOnlyList<IconInfo> SourceIcons { get; } = Enum.GetValues<Icon>()
        .Select(icon => new IconInfo { Name = icon.ToString(), Value = icon })
        .ToImmutableArray();

    [ObservableProperty]
    public partial IReadOnlyList<IconInfo> Icons { get; set; } = SourceIcons;

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Xaml), nameof(XamlExtension), nameof(CSharp), nameof(CSharpMarkup))]
    public partial bool UsesSymbol { get; set; } = false;
    [ObservableProperty]
    public partial FlowDirection FlowDirection { get; set; } = FlowDirection.LeftToRight;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Xaml), nameof(XamlExtension), nameof(CSharp), nameof(CSharpMarkup))]
    public partial IconVariant IconVariant { get; set; } = IconVariant.Regular;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Xaml), nameof(XamlExtension), nameof(CSharp), nameof(CSharpMarkup))]
    public partial IconInfo Selected { get; set; } = SourceIcons[0].Let(x => x.IsSelected = true);

    private string Prefix => UsesSymbol ? "Symbol" : "Fluent";
    private string Property => UsesSymbol ? "Symbol" : "Icon";
    public string Xaml => IconVariant == IconVariant.Regular
        ? $"<ic:{Prefix}Icon {Property}=\"{Selected?.Name}\" />"
        : $"<ic:{Prefix}Icon {Property}=\"{Selected?.Name}\" IconVariant=\"{IconVariant}\" />";
    public string XamlExtension => IconVariant == IconVariant.Regular
        ? $"{{ic:{Prefix}Icon {Property}={Selected?.Name}}}"
        : $"{{ic:{Prefix}Icon {Property}={Selected?.Name}, IconVariant={IconVariant}}}";
    public string CSharp => IconVariant == IconVariant.Regular
        ? $"new {Prefix}Icon {{ {Property} = {Property}.{Selected?.Name} }};"
        : $"new {Prefix}Icon {{ {Property} = {Property}.{Selected?.Name}, IconVariant = IconVariant.{IconVariant} }};";
    public string CSharpMarkup => IconVariant == IconVariant.Regular
        ? $"new {Prefix}Icon().{Property}({Property}.{Selected?.Name})"
        : $"new {Prefix}Icon().{Property}({Property}.{Selected?.Name}).IconVariant(IconVariant.{IconVariant})";

    [RelayCommand]
    public void SelectIcon(IconInfo icon)
    {
        Selected?.IsSelected = false;
        icon.IsSelected = true;
        Selected = icon;
    }

    [RelayCommand]
    public void CopyXaml() => App.Clipboard?.SetTextAsync(Xaml);
    [RelayCommand]
    public void CopyXamlExtension() => App.Clipboard?.SetTextAsync(XamlExtension);
    [RelayCommand]
    public void CopyCSharp() => App.Clipboard?.SetTextAsync(CSharp);
    [RelayCommand]
    public void CopyCSharpMarkup() => App.Clipboard?.SetTextAsync(CSharpMarkup);

    public string Version => typeof(Icon).Assembly.GetName().Version?.ToString(3) ?? "–";

    public void RefreshIcons()
    {
        if (string.IsNullOrEmpty(SearchText))
        {
            Icons = UsesSymbol ? SourceIcons.Where(x => x.HasSymbol).ToImmutableArray() : SourceIcons;
        }
        else
        {
            Icons = SourceIcons
                .Where(x => (!UsesSymbol || x.HasSymbol) && x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToImmutableArray();
        }
    }
    async partial void OnSearchTextChanged(string value)
    {
        await Task.Yield();
        RefreshIcons();
    }
}
