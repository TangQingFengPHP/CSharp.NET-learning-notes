# 06 原理解析与源码分析

## 详解

这个项目不再强调“生成什么代码”，而是解释 Source Generator 在编译器里的位置：

- 编译前端产生语法树和语义模型
- 生成器读取这些输入
- 生成器产出新的 `.g.cs`
- 编译器把原始代码和生成代码一起编译

## 你应该关注的源码角色

- `Compilation`
- `SyntaxNode`
- `SemanticModel`
- `GeneratorExecutionContext`
- `IncrementalGeneratorInitializationContext`
- `SourceProductionContext`
