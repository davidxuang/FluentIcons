namespace FluentIcons.Gallery.Models;

public record class IconInfo(Icon Value)
{
    public string Name { get; init; } = Value.ToString();

    public bool IsSymbolAvailable => Enum.IsDefined((Symbol)(int)Value);
}
