# FluentIcons

A multi-framework wrapper of [fluentui-system-icons](https://github.com/microsoft/fluentui-system-icons).

## Packages

| Package                                                                                                                                                                                    | Platform                                                                                                                                                                                                                                                                                                                   |
| ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [![FluentIcons.Common](https://img.shields.io/nuget/v/FluentIcons.Common?label=FluentIcons.Common)](https://www.nuget.org/packages/FluentIcons.Common)                                     |                                                                                                                                                                                                                                                                                                                            |
| [![FluentIcons.Avalonia](https://img.shields.io/nuget/v/FluentIcons.Avalonia?label=FluentIcons.Avalonia)](https://www.nuget.org/packages/FluentIcons.Avalonia)                             | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia.svg) Avalonia 11](https://www.nuget.org/packages/Avalonia/11.0.0)                                                                                                                                                                           |
| [![FluentIcons.Avalonia.Fluent](https://img.shields.io/nuget/v/FluentIcons.Avalonia.Fluent?label=FluentIcons.Avalonia.Fluent)](https://www.nuget.org/packages/FluentIcons.Avalonia.Fluent) | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia-fluent.svg) FluentAvalonia 2](https://www.nuget.org/packages/FluentAvaloniaUI/2.0.0) (Avalonia 11)                                                                                                                                          |
| [![FluentIcons.Maui](https://img.shields.io/nuget/v/FluentIcons.Maui?label=FluentIcons.Maui)](https://www.nuget.org/packages/FluentIcons.Maui)                                             | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/maui.svg) MAUI 8](https://www.nuget.org/packages/Microsoft.Maui.Sdk/8.0.3)                                                                                                                                                                           |
| [![FluentIcons.Uwp](https://img.shields.io/nuget/v/FluentIcons.Uwp?label=FluentIcons.Uwp)](https://www.nuget.org/packages/FluentIcons.Uwp)                                                 | ![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/windows-10.svg) UWP 10.0.10773 <br/> [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/uno.svg) Uno.UI 5](https://www.nuget.org/packages/Uno.UI/5.0.19)                                                                           |
| [![FluentIcons.WinUI](https://img.shields.io/nuget/v/FluentIcons.WinUI?label=FluentIcons.WinUI)](https://www.nuget.org/packages/FluentIcons.WinUI)                                         | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/wasdk.svg) Windows App SDK 1.2](https://www.nuget.org/packages/Microsoft.WindowsAppSDK/1.2.221109.1) <br/> [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/uno.svg) Uno.WinUI 5](https://www.nuget.org/packages/Uno.UI/5.0.19) |
| [![FluentIcons.WPF](https://img.shields.io/nuget/v/FluentIcons.WPF?label=FluentIcons.WPF)](https://www.nuget.org/packages/FluentIcons.WPF)                                                 | ![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/dotnet.svg) .NET Framework 4.6.2 <br/> ![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/dotnet.svg) .NET 6                                                                                                                        |

### Legacy

| Ref                                                                                       | Package                       | Platform                                                                                                                                                                              |
| ----------------------------------------------------------------------------------------- | ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [avalonia-v0.10](https://github.com/davidxuang/FluentIcons/tree/backports/avalonia-v0.10) | `FluentIcons.Avalonia`        | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia.svg) Avalonia 0.10](https://www.nuget.org/packages/Avalonia/0.10.0)                                    |
| [avalonia-v0.10](https://github.com/davidxuang/FluentIcons/tree/backports/avalonia-v0.10) | `FluentIcons.Avalonia.Fluent` | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia-fluent.svg) FluentAvalonia 1.3](https://www.nuget.org/packages/FluentAvaloniaUI/1.3.0) (Avalonia 0.10) |

## Usage

```xml
<Window xmlns:ic="using:FluentIcons.WinUI">
<!-- or FluentIcons.Avalonia / FluentIcons.Avalonia.Fluent / FluentIcons.Maui / FluentIcons.WPF -->
    <ic:SymbolIcon Symbol="ArrowLeft" IsFilled="True" />
</Window>
```

This package features `<SymbolIcon>` element, and `<SymbolIconSource>` on platforms with `<IconSource>`, which generally provide following properties:

-   **Symbol** : [Symbol](./FluentIcons.Common/Symbol.cs)
    -   _Breaking change since 1.1.229: LTR/RTL specific values are removed, use `FlowDirection` instead._
-   **IsFilled** : bool
-   **UseSegoeMetrics**: bool
    -   _New feature since 1.1.229: match the metrics of [Segoe Fluent Icons](https://learn.microsoft.com/en-us/windows/apps/design/style/segoe-fluent-icons-font). see also: [Seagull Icons](./seagull-icons/README.md)_
-   **FlowDirection** : FlowDirection
    -   _New feature since 1.1.229: switch between LTR/RTL icon variant._
-   **FontSize** : double
    -   _Breaking change since 1.1.225: no longer inherit value from parent element to match WinUI behaviours._
-   **Foreground** : Brush

```xml
<Window xmlns:ic="using:FluentIcons.WinUI">
    <Expander Header="{ic:SymbolIconExtension Symbol=ArrowLeft}" />
</Window>
```

`SymbolIconExtension` and `SymbolIconSourceExtension` have been added since 1.1.242. These extensions will auto-detect `FlowDirection` from parent control, except on (non-Uno) UWP where `IXamlServiceProvider` is not available.

### Avalonia

To enable `UseSegoeMetrics` globally, call `UseSegoeMetric(this AppBuilder builder)`. Markup extension classes are in a child namespace to stop style selectors from throwing for their naming conventions.

### MAUI

⚠️ The extension method `UseFluentIcons(this MauiAppBuilder builder, bool useSegoeMetrics)` must be called to register fonts properly. ⚠️

`<SymbolImageSource>` and `SymbolImageSourceExtension` are provided on MAUI as stand-ins.

### UWP / WinUI

⚠️ You must reference this package directly so that fonts can be included in the build output properly. ⚠️

To enable `UseSegoeMetrics` globally, call `UseSegoeMetric(this Application app)` or `UseSegoeMetric(this IHostBuilder builder)` (WinUI-only).

### WPF

To enable `UseSegoeMetrics` globally, call `UseSegoeMetric(this Application app)`.
