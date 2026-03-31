namespace SourceGenerator06.Principle.SourceAnalysis;

public sealed class SourceGeneratorRoles
{
    public IReadOnlyList<string> GetRoles()
    {
        return
        [
            "Compilation: 提供整个编译单元的全局视图。",
            "SyntaxNode: 提供原始语法结构。",
            "SemanticModel: 提供类型和符号分析能力。",
            "GeneratorExecutionContext: 传统生成器的主要入口。",
            "IncrementalGeneratorInitializationContext: 增量生成器的配置入口。",
            "SourceProductionContext: 最终输出生成代码的上下文。"
        ];
    }
}
