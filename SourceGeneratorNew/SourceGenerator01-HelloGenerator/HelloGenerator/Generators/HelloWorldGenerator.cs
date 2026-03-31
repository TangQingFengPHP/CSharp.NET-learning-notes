using HelloGenerator.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace HelloGenerator.Generators;

[Generator]
public sealed class HelloWorldGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var source = HelloWorldSourceBuilder.Build();
        context.AddSource("HelloMessages.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
