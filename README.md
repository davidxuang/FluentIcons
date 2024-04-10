# FluentIcons

**_This is a backports release for Avalonia 0.10.\*._**

A multi-framework wrapper of [fluentui-system-icons](https://github.com/microsoft/fluentui-system-icons).

## Packages

| Package                                                                                                                                                          | Platform                                                                                                                                                                            |
| ---------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [![FluentIcons.Avalonia](https://badgen.net/badge/FluentIcons.Avalonia/v1.1.235)](https://www.nuget.org/packages/FluentIcons.Avalonia/1.1.235)                   | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia.svg) Avalonia 10](https://www.nuget.org/packages/Avalonia/0.10.0)                                    |
| [![FluentIcons.Avalonia.Fluent](https://badgen.net/badge/FluentIcons.Avalonia.Fluent/v1.1.235)](https://www.nuget.org/packages/FluentIcons.Avalonia.Fluent/1.1.235) | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia-fluent.svg) FluentAvalonia 1.3](https://www.nuget.org/packages/FluentAvaloniaUI/1.3.0) (Avalonia 10) |

## Usage

```xml
<Window xmlns:ic="using:FluentIcons.Avalonia">
<!-- or FluentIcons.Avalonia.Fluent / FluentIcons.Maui / FluentIcons.WinUI / FluentIcons.WPF -->
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

### Avalonia / WPF

To enable `UseSegoeMetrics` globally, call `UseSegoeMetric(this AppBuilder builder)` on Avalonia and `UseSegoeMetric(this Application app)` on WPF.

### MAUI

⚠️ The extension method `UseFluentIcons(this MauiAppBuilder builder, bool useSegoeMetrics)` must be called to register fonts properly. ⚠️

`SymbolImageSource` is provided on MAUI.

### UWP / WinUI

⚠️ You must reference this package directly so that fonts can be included in the build output properly. ⚠️

To enable `UseSegoeMetrics` globally, call `UseSegoeMetric(this Application app)` or `UseSegoeMetric(this IHostBuilder builder)` (WinUI-only).
