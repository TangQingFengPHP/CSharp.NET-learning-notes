using System.Text;
using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace FeatureFlagGenerator.Generators;

[Generator]
public sealed class FeatureFlagSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var textFiles = context.AdditionalTextsProvider
            .Where(static file => Path.GetFileName(file.Path).Equals("feature-flags.txt", StringComparison.OrdinalIgnoreCase))
            .Select(static (file, token) => file.GetText(token)?.ToString() ?? string.Empty);

        context.RegisterSourceOutput(textFiles, (productionContext, fileContent) =>
        {
            var flags = fileContent
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#", StringComparison.Ordinal))
                .ToArray();

            var members = string.Join(Environment.NewLine, flags.Select(flag => $"    public const string {ToIdentifier(flag)} = \"{flag}\";"));
            var allFlags = string.Join(", ", flags.Select(flag => $"{ToIdentifier(flag)}"));
            var source = $@"
namespace Generated
{{
    public static class FeatureFlags
    {{
{members}

        public static readonly string[] All = new[] {{ {allFlags} }};
    }}
}}";

            productionContext.AddSource("FeatureFlags.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }

    private static string ToIdentifier(string value)
    {
        var chars = value.Where(char.IsLetterOrDigit).ToArray();
        var normalized = new string(chars);
        return string.IsNullOrWhiteSpace(normalized) ? "UnknownFlag" : normalized;
    }
}
