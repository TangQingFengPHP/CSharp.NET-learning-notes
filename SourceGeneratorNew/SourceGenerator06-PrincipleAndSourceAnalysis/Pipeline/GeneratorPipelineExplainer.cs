using SourceGenerator06.Principle.Models;

namespace SourceGenerator06.Principle.Pipeline;

public sealed class GeneratorPipelineExplainer
{
    public IReadOnlyList<CompilerStage> GetStages()
    {
        return
        [
            new() { Order = 1, Name = "Parse", Description = "编译器先把源码解析成语法树。" },
            new() { Order = 2, Name = "Semantic", Description = "语义模型提供符号、类型、Attribute 等信息。" },
            new() { Order = 3, Name = "Generator", Description = "Source Generator 读取语法/语义输入并生成新的源码。" },
            new() { Order = 4, Name = "AddSource", Description = "生成出的 .g.cs 被补进本次编译。" },
            new() { Order = 5, Name = "Emit", Description = "原始代码和生成代码一起被编译成程序集。" }
        ];
    }
}
