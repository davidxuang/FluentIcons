using System;
using System.Diagnostics;

namespace FluentIcons.Common.Internals;

[Conditional("__NEVER__")]
[AttributeUsage(AttributeTargets.Field)]
internal sealed class NonResizableAttribute : Attribute { }
