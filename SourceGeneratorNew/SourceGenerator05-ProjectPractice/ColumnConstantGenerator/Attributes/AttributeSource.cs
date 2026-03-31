namespace ColumnConstantGenerator.Attributes;

internal static class AttributeSource
{
    public const string Code = @"
using System;

namespace ColumnMetadata
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GenerateColumnConstantsAttribute : Attribute
    {
    }
}";
}
