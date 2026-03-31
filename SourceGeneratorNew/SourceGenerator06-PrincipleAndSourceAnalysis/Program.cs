using SourceGenerator06.Principle.Pipeline;
using SourceGenerator06.Principle.SourceAnalysis;

var pipelineExplainer = new GeneratorPipelineExplainer();
var roles = new SourceGeneratorRoles();

Console.WriteLine("=== 06 Principle And Source Analysis ===");
Console.WriteLine();
Console.WriteLine("编译流程:");
foreach (var stage in pipelineExplainer.GetStages())
{
    Console.WriteLine($"{stage.Order}. {stage.Name} => {stage.Description}");
}

Console.WriteLine();
Console.WriteLine("源码角色:");
foreach (var role in roles.GetRoles())
{
    Console.WriteLine($"- {role}");
}
