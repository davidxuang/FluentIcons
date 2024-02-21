# FluentIcons

A multi-framework wrapper of [fluentui-system-icons](https://github.com/microsoft/fluentui-system-icons).

## Packages

| Package                       | Version                                                                                                                                               |
| ----------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| `FluentIcons.Common`          | [![FluentIcons.Common](https://badgen.net/nuget/v/FluentIcons.Common)](https://www.nuget.org/packages/FluentIcons.Common/)                            |
| `FluentIcons.Avalonia`        | [![FluentIcons.Avalonia](https://badgen.net/nuget/v/FluentIcons.Avalonia)](https://www.nuget.org/packages/FluentIcons.Avalonia/)                      |
| `FluentIcons.Avalonia.Fluent` | [![FluentIcons.Avalonia.Fluent](https://badgen.net/nuget/v/FluentIcons.Avalonia.Fluent)](https://www.nuget.org/packages/FluentIcons.Avalonia.Fluent/) |
| `FluentIcons.WinUI`           | [![FluentIcons.WinUI](https://badgen.net/nuget/v/FluentIcons.WinUI)](https://www.nuget.org/packages/FluentIcons.WinUI/)                               |
| `FluentIcons.WPF`             | [![FluentIcons.WPF](https://badgen.net/nuget/v/FluentIcons.WPF)](https://www.nuget.org/packages/FluentIcons.WPF/)                                     |

### Legacy versions

| Avalonia 0.10.* [backports](https://github.com/davidxuang/FluentIcons/tree/avalonia-v0.10) |                                                                                                                                       |
| ------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------- |
| `FluentIcons.Avalonia`                                                                     | [![FluentIcons.Avalonia](https://badgen.net/badge/nuget/v1.1.203)](https://www.nuget.org/packages/FluentIcons.Avalonia/1.1.203)       |
| `FluentIcons.FluentAvalonia`                                                               | [![FluentIcons.Avalonia](https://badgen.net/badge/nuget/v1.1.203)](https://www.nuget.org/packages/FluentIcons.FluentAvalonia/1.1.203) |

## Usage

```xml
<Window xmlns:ic="using:FluentIcons.Avalonia"> <!-- or FluentIcons.Avalonia.Fluent / FluentIcons.WinUI / FluentIcons.WPF -->
    <ic:SymbolIcon Symbol="ArrowLeft" IsFilled="True" />
</Window>
```

This package features `<SymbolIcon>` element, and `<SymbolIconSource>` on platforms with `<IconSource>`, which generally provide following properties:

- **Symbol** : [Symbol](./FluentIcons.Common/Symbol.cs)
    - *Breaking change since 1.1.229: LTR/RTL specific values are removed, use `FlowDirection` instead.*
- **IsFilled** : bool
- **UseSegoeMetrics**: bool
    - *New feature since 1.1.229: match the metrics of [Segoe Fluent Icons](https://learn.microsoft.com/en-us/windows/apps/design/style/segoe-fluent-icons-font). see also: [Seagull Icons](./seagull-icons/README.md)*
- **FlowDirection** : FlowDirection
    - *New feature since 1.1.229: switch between LTR/RTL icon variant.*
- **FontSize** : double
    - *Breaking change since 1.1.225: no longer inherit value from parent element to match WinUI behaviours.*
- **Foreground** : Brush

You may also enable `UseSegoeMetrics` globally using the extension method `UseSegoeMetrics()` provided for `IHostBuilder` / `AppBuilder` (Avalonia) / `Application` (WPF).
