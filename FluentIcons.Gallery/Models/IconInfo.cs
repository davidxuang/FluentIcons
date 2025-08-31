namespace FluentIcons.Gallery.Models;

public sealed partial class IconInfo : ObservableObject, IComparable<IconInfo>
{
    public required string Name { get; init; }
    public required Icon Value { get; init; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public Symbol SymbolValue => (Symbol)(int)Value;

    public int CompareTo(IconInfo? other) => Value.CompareTo(other?.Value ?? (Icon)int.MaxValue);
}
