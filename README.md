# FluentIcons

A multi-framework control library of [fluentui-system-icons](https://github.com/microsoft/fluentui-system-icons).

## Packages

| Package                                                                                                                                                                                    | Platform                                                                                                                                                                                                                                                                                                                        |
| ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [![FluentIcons.Common](https://img.shields.io/nuget/v/FluentIcons.Common?label=FluentIcons.Common)](https://www.nuget.org/packages/FluentIcons.Common)                                     | _meta package_                                                                                                                                                                                                                                                                                                                  |
| [![FluentIcons.Uno](https://img.shields.io/nuget/v/FluentIcons.Uno?label=FluentIcons.Uno)](https://www.nuget.org/packages/FluentIcons.Uno)                                                 | _meta package_                                                                                                                                                                                                                                                                                                                  |
| [![FluentIcons.Avalonia](https://img.shields.io/nuget/v/FluentIcons.Avalonia?label=FluentIcons.Avalonia)](https://www.nuget.org/packages/FluentIcons.Avalonia)                             | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia.svg) Avalonia 11](https://www.nuget.org/packages/Avalonia/11.0.0)                                                                                                                                                                                |
| [![FluentIcons.Avalonia.Fluent](https://img.shields.io/nuget/v/FluentIcons.Avalonia.Fluent?label=FluentIcons.Avalonia.Fluent)](https://www.nuget.org/packages/FluentIcons.Avalonia.Fluent) | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia-fluent.svg) FluentAvalonia 2](https://www.nuget.org/packages/FluentAvaloniaUI/2.0.0) (Avalonia 11)                                                                                                                                               |
| [![FluentIcons.Maui](https://img.shields.io/nuget/v/FluentIcons.Maui?label=FluentIcons.Maui)](https://www.nuget.org/packages/FluentIcons.Maui)                                             | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/maui.svg) MAUI 8](https://www.nuget.org/packages/Microsoft.Maui.Sdk/8.0.3)                                                                                                                                                                                |
| [![FluentIcons.Uwp](https://img.shields.io/nuget/v/FluentIcons.Uwp?label=FluentIcons.Uwp)](https://www.nuget.org/packages/FluentIcons.Uwp)                                                 | ![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/windows-10.svg) UWP 10.0.10773 <br/> [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/uno.svg) Uno.UI 5.4](https://www.nuget.org/packages/Uno.UI/5.4.22)                                                                              |
| [![FluentIcons.WinUI](https://img.shields.io/nuget/v/FluentIcons.WinUI?label=FluentIcons.WinUI)](https://www.nuget.org/packages/FluentIcons.WinUI)                                         | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/wasdk.svg) Windows App SDK 1.2](https://www.nuget.org/packages/Microsoft.WindowsAppSDK/1.2.221109.1) <br/> [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/uno.svg) Uno.WinUI 5.4](https://www.nuget.org/packages/Uno.WinUI/5.4.22) |
| [![FluentIcons.Wpf](https://img.shields.io/nuget/v/FluentIcons.Wpf?label=FluentIcons.Wpf)](https://www.nuget.org/packages/FluentIcons.Wpf)                                                 | ![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/dotnet.svg) .NET Framework 4.6.2 <br/> ![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/dotnet.svg) .NET 6                                                                                                                             |

### Legacy

| Ref                                                                                       | Package                       | Platform                                                                                                                                                                              |
| ----------------------------------------------------------------------------------------- | ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [avalonia-v0.10](https://github.com/davidxuang/FluentIcons/tree/backports/avalonia-v0.10) | `FluentIcons.Avalonia`        | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia.svg) Avalonia 0.10](https://www.nuget.org/packages/Avalonia/0.10.0)                                    |
| [avalonia-v0.10](https://github.com/davidxuang/FluentIcons/tree/backports/avalonia-v0.10) | `FluentIcons.Avalonia.Fluent` | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia-fluent.svg) FluentAvalonia 1.3](https://www.nuget.org/packages/FluentAvaloniaUI/1.3.0) (Avalonia 0.10) |
| [uno-v5.0](https://github.com/davidxuang/FluentIcons/tree/backports/uno-v5.0)             | `FluentIcons.Uwp`             | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/uno.svg) Uno.UI 5.0](https://www.nuget.org/packages/Uno.UI/5.0.19)                                              |
| [uno-v5.0](https://github.com/davidxuang/FluentIcons/tree/backports/uno-v5.0)             | `FluentIcons.WinUI`           | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/uno.svg) Uno.WinUI 5.0](https://www.nuget.org/packages/Uno.WinUI/5.0.19)                                        |

## Usage

```xml
<Window xmlns:ic="using:FluentIcons.WinUI">
<!-- or FluentIcons.Avalonia / FluentIcons.Avalonia.Fluent / FluentIcons.Maui / FluentIcons.Wpf -->
    <ic:FluentIcon Icon="ArrowLeft" IconVariant="Regular" IconSize="Size32" />
    <ic:SymbolIcon Symbol="Calendar" IconVariant="Color" />
</Window>
```

This package features `<FluentIcon>`/`<SymbolIcon>` element, and `<FluentIconSource>`/`<SymbolIconSource>` on platforms with `<IconSource>`, which generally provide following properties:

-   **Icon** _(Fluent...)_ / **Symbol** _(Symbol...)_ : [Icon](./FluentIcons.Common/Icon.cs) / [Symbol](./FluentIcons.Common/Symbol.cs)
-   **IconVariant** : [IconVariant](./FluentIcons.Common/IconVariant.cs)
    -   _New in version 1.1.278: `Color` variant added along with [COLRv1](https://learn.microsoft.com/en-us/typography/opentype/spec/colr) migration._
-   **IconSize** _(Fluent...)_ : [IconSize](./FluentIcons.Common/IconSize.cs)
-   **FlowDirection** : FlowDirection
    -   _Switch between LTR/RTL icon variant._
-   **FontSize** : double
-   **Foreground** : Brush

The _Fluent_ variant provides all sizes of icons untouched compared to upstream, while the _Symbol_ variant mimics the APIs and appearances of `SymbolIcon` and [Segoe Fluent Icons](https://learn.microsoft.com/en-us/windows/apps/design/style/segoe-fluent-icons-font) from WinUI, which is powered by a derived version from the child project [Seagull Icons](./seagull-icons/README.md). These variants were controlled by the deprecated `UseSegoeMetrics` property in distributions prior to 1.1.278.

```xml
<Window xmlns:ic="using:FluentIcons.WinUI">
    <Expander Header="{ic:SymbolIcon Symbol=ArrowLeft}" />
</Window>
```

Markup extension classes have been added since 1.1.242. These extensions will bind their `FlowDirection` to that of the parent control, except `FluentIconSourceExtension`/`SymbolIconSourceExtension` on (non-Uno) UWP where `IXamlServiceProvider` is not available.

### Avalonia

Markup extension classes are in a child namespace to stop style selectors from throwing for their naming conventions.

### MAUI

⚠️ The extension method `UseFluentIcons(this MauiAppBuilder builder)` must be called to register fonts properly. ⚠️

`<SymbolImageSource>` and `SymbolImageSourceExtension` are provided on MAUI as stand-ins.

## Known issues

Color icons are broken on several platforms on WPF, because of the lack of [COLR](https://learn.microsoft.com/en-us/typography/opentype/spec/colr) rendering support. It is also not working in environments like macOS and WebAssembly when rendering with SkiaSharp 2, possibly affecting Avalonia and Uno.
