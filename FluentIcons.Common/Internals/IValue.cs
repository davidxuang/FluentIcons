using System;

namespace FluentIcons.Common.Internals;

public interface IValue<T>
    where T : Enum
{
    internal abstract T Value { get; set; }
}
