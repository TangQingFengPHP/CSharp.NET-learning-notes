namespace EndpointGenerator.Attributes;

internal static class AttributeSource
{
    public const string Code = @"
using System;

namespace EndpointMetadata
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GenerateEndpointAttribute : Attribute
    {
        public GenerateEndpointAttribute(string method, string route)
        {
            Method = method;
            Route = route;
        }

        public string Method { get; }
        public string Route { get; }
    }
}";
}
