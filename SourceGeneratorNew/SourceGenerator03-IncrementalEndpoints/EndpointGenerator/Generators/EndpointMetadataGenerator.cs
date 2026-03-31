using System.Text;
using System.Linq;
using EndpointGenerator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EndpointGenerator.Generators;

[Generator]
public sealed class EndpointMetadataGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(i => i.AddSource("GenerateEndpointAttribute.g.cs", SourceText.From(AttributeSource.Code, Encoding.UTF8)));

        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax classDeclaration && classDeclaration.AttributeLists.Count > 0,
                static (context, _) => GetCandidate(context))
            .Where(static result => result is not null);

        context.RegisterSourceOutput(candidates, static (productionContext, candidate) =>
        {
            if (candidate is null)
            {
                return;
            }

            productionContext.AddSource(candidate.Value.HintName, SourceText.From(candidate.Value.Source, Encoding.UTF8));
        });
    }

    private static GeneratedSource? GetCandidate(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
        {
            return null;
        }

        var attribute = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "EndpointMetadata.GenerateEndpointAttribute");
        if (attribute is null || attribute.ConstructorArguments.Length != 2)
        {
            return null;
        }

        var method = attribute.ConstructorArguments[0].Value?.ToString() ?? "GET";
        var route = attribute.ConstructorArguments[1].Value?.ToString() ?? "/";
        var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : classSymbol.ContainingNamespace.ToDisplayString();
        var namespaceStart = string.IsNullOrWhiteSpace(namespaceName) ? string.Empty : $"namespace {namespaceName};";
        var source = $@"
{namespaceStart}

public partial class {classSymbol.Name}
{{
    public const string Method = ""{method}"";
    public const string Route = ""{route}"";

    public static string Describe() => $""{{Method}} {{Route}}"";
}}";

        return new GeneratedSource($"{classSymbol.Name}.EndpointMetadata.g.cs", source);
    }

    private struct GeneratedSource
    {
        public GeneratedSource(string hintName, string source)
        {
            HintName = hintName;
            Source = source;
        }

        public string HintName { get; }

        public string Source { get; }
    }
}
