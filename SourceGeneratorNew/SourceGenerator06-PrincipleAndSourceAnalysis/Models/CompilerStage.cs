namespace SourceGenerator06.Principle.Models;

public sealed class CompilerStage
{
    public required int Order { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }
}
