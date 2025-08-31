namespace FluentIcons.Gallery.Helpers;

public static class FunctionalHelpers
{
    public static T Let<T>(this T value, Action<T> action)
    {
        action(value);
        return value;
    }
}
