# FluentIcons

**_This is a backports release for Avalonia 0.10.\*._**

A multi-framework wrapper of [fluentui-system-icons](https://github.com/microsoft/fluentui-system-icons).

## Packages

| Package                                                                                                                                                          | Platform                                                                                                                                                                            |
| ---------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [![FluentIcons.Avalonia](https://badgen.net/badge/FluentIcons.Avalonia/v1.1.203)](https://www.nuget.org/packages/FluentIcons.Avalonia/1.1.203)                   | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia.svg) Avalonia 10](https://www.nuget.org/packages/Avalonia/0.10.0)                                    |
| [![FluentIcons.FluentAvalonia](https://badgen.net/badge/FluentIcons.FluentAvalonia/v1.1.203)](https://www.nuget.org/packages/FluentIcons.FluentAvalonia/1.1.203) | [![](https://cdn.jsdelivr.net/gh/davidxuang/FluentIcons@static/assets/avalonia-fluent.svg) FluentAvalonia 1.3](https://www.nuget.org/packages/FluentAvaloniaUI/1.3.0) (Avalonia 10) |

## Usage

```xml
<Window xmlns:ic="using:FluentIcons.Avalonia"> <!-- or FluentIcons.FluentAvalonia / FluentIcons.WPF -->
    <ic:SymbolIcon Symbol="ArrowLeft" IsFilled="True" />
</Window>
```
