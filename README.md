# FluentIcons

A multi-framework wrapper of [fluentui-system-icons](https://github.com/microsoft/fluentui-system-icons).

## Packages

| Package              |                                                             Version                                                              |
| -------------------- | :------------------------------------------------------------------------------------------------------------------------------: |
| FluentIcons.Common   |    [![FluentIcons.Common](https://badgen.net/nuget/v/FluentIcons.Common)](https://www.nuget.org/packages/FluentIcons.Common/)    |
| FluentIcons.Avalonia | [![FluentIcons.Avalonia](https://badgen.net/nuget/v/FluentIcons.Avalonia)](https://www.nuget.org/packages/FluentIcons.Avalonia/) |
| FluentIcons.WPF      |        [![FluentIcons.WPF](https://badgen.net/nuget/v/FluentIcons.WPF)](https://www.nuget.org/packages/FluentIcons.WPF/)         |

## Usage

```xml
<Window xmlns:ic="using:FluentIcons.Avalonia"> <!-- or FluentIcons.WPF -->
    <ic:SymbolIcon Symbol="ArrowLeft" IsFilled="True" />
</Window>
```
