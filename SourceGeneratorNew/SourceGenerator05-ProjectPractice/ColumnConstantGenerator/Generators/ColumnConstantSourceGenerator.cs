using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using ColumnConstantGenerator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ColumnConstantGenerator.Generators;

[Generator]
public sealed class ColumnConstantSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(i => i.AddSource("GenerateColumnConstantsAttribute.g.cs", SourceText.From(AttributeSource.Code, Encoding.UTF8)));
        context.RegisterForSyntaxNotifications(() => new CandidateReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not CandidateReceiver receiver)
        {
            return;
        }

        foreach (var classDeclaration in receiver.Candidates)
        {
            var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            if (model.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
            {
                continue;
            }

            if (!classSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "ColumnMetadata.GenerateColumnConstantsAttribute"))
            {
                continue;
            }

            var properties = classSymbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.DeclaredAccessibility == Accessibility.Public).ToArray();
            var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : classSymbol.ContainingNamespace.ToDisplayString();
            var namespaceStart = string.IsNullOrWhiteSpace(namespaceName) ? string.Empty : $"namespace {namespaceName};";
            var columnClassName = classSymbol.Name + "Columns";
            var members = string.Join(Environment.NewLine, properties.Select(p => $"    public const string {p.Name} = \"{p.Name}\";"));
            var all = string.Join(", ", properties.Select(p => p.Name));
            var source = $@"
{namespaceStart}

public static class {columnClassName}
{{
{members}

    public static readonly string[] All = new[] {{ {all} }};
}}";

            context.AddSource($"{columnClassName}.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private sealed class CandidateReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> Candidates { get; } = [];

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclaration && classDeclaration.AttributeLists.Count > 0)
            {
                Candidates.Add(classDeclaration);
            }
        }
    }
}
