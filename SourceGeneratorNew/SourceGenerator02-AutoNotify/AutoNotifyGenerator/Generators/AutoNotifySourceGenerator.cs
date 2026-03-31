using System.Text;
using System.Linq;
using AutoNotifyGenerator.Attributes;
using AutoNotifyGenerator.Receivers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AutoNotifyGenerator.Generators;

[Generator]
public sealed class AutoNotifySourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(i => i.AddSource("AutoNotifyAttribute.g.cs", SourceText.From(AttributeSource.Code, Encoding.UTF8)));
        context.RegisterForSyntaxNotifications(() => new FieldSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not FieldSyntaxReceiver receiver)
        {
            return;
        }

        foreach (var field in receiver.CandidateFields)
        {
            var model = context.Compilation.GetSemanticModel(field.SyntaxTree);
            if (model.GetDeclaredSymbol(field.Declaration.Variables[0]) is not IFieldSymbol fieldSymbol)
            {
                continue;
            }

            if (!fieldSymbol.GetAttributes().Any(attribute => attribute.AttributeClass?.ToDisplayString() == "AutoNotify.AutoNotifyAttribute"))
            {
                continue;
            }

            var classSymbol = fieldSymbol.ContainingType;
            var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : classSymbol.ContainingNamespace.ToDisplayString();
            var fieldName = fieldSymbol.Name;
            var propertyName = BuildPropertyName(fieldName);
            var source = BuildSource(namespaceName, classSymbol.Name, fieldName, propertyName, fieldSymbol.Type.ToDisplayString());

            context.AddSource($"{classSymbol.Name}.{propertyName}.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static string BuildPropertyName(string fieldName)
    {
        var trimmed = fieldName.TrimStart('_');
        return string.IsNullOrWhiteSpace(trimmed)
            ? "GeneratedProperty"
            : char.ToUpperInvariant(trimmed[0]) + trimmed.Substring(1);
    }

    private static string BuildSource(string namespaceName, string className, string fieldName, string propertyName, string typeName)
    {
        var namespaceStart = string.IsNullOrWhiteSpace(namespaceName) ? string.Empty : $"namespace {namespaceName};";
        return $@"
{namespaceStart}

public partial class {className}
{{
    public {typeName} {propertyName}
    {{
        get => {fieldName};
        set => {fieldName} = value;
    }}
}}";
    }
}
