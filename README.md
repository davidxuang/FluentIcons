# FluentIcons

A multi-framework wrapper of [fluentui-system-icons](https://github.com/microsoft/fluentui-system-icons).

`Symbol` enum was built per the regular variant; thus [some mappings](./FluentIcons.Common/Internals/SymbolConversion.cs#L14) has been made for using filled symbols.

## Packages

| Package              |                                 Version                                  |
| -------------------- | :----------------------------------------------------------------------: |
| FluentIcons.Common   |   ![FluentIcons.Common](https://badgen.net/nuget/v/FluentIcons.Common)   |
| FluentIcons.Avalonia | ![FluentIcons.Avalonia](https://badgen.net/nuget/v/FluentIcons.Avalonia) |
| FluentIcons.WPF      |      ![FluentIcons.WPF](https://badgen.net/nuget/v/FluentIcons.WPF)      |

## Usage

```xml
<Window xmlns:ic="using:FluentIcons.Avalonia"> <!-- or FluentIcons.WPF -->
    <ic:SymbolIcon Symbol="ArrowLeft" IsFilled="True" />
</Window>
```
