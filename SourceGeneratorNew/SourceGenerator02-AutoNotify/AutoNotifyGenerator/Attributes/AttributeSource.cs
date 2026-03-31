namespace AutoNotifyGenerator.Attributes;

internal static class AttributeSource
{
    public const string Code = @"
using System;

namespace AutoNotify
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class AutoNotifyAttribute : Attribute
    {
    }
}";
}
