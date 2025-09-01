using Avalonia.Controls;
using Avalonia.Input.Platform;
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
    [NotifyPropertyChangedFor(nameof(Xaml), nameof(XamlExtension), nameof(CSharp), nameof(CSharpMarkup))]
    public partial bool UsesSymbol { get; set; } = false;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Xaml), nameof(XamlExtension), nameof(CSharp), nameof(CSharpMarkup))]
    public partial IconVariant IconVariant { get; set; } = IconVariant.Regular;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Xaml), nameof(XamlExtension), nameof(CSharp), nameof(CSharpMarkup))]
    public partial IconInfo Selected { get; set; } = Icons[0].Let(x => x.IsSelected = true);

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
