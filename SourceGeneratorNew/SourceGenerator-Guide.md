# C# .NET Source Generator 源生成器专题

本专题沿用之前的学习目录风格，但针对源生成器的特点做了更清晰的拆分：一个知识点一个文件夹，且在每个知识点内部按职责继续拆分为 `Generator`、`Consumer`、`Models`、`Templates` 等子目录，而不是把所有类型都堆在 `Program.cs` 中。

## 目录

1. `SourceGenerator01-HelloGenerator`
   - 最小可运行源生成器
   - 生成固定代码
   - Generator / Consumer 关系
2. `SourceGenerator02-AutoNotify`
   - 基于 Attribute 的代码生成
   - 自动生成属性
   - `SyntaxReceiver` / 语义分析
3. `SourceGenerator03-IncrementalEndpoints`
   - `IIncrementalGenerator`
   - 增量筛选候选节点
   - 生成 Endpoint 元数据常量
4. `SourceGenerator04-AdditionalFiles`
   - 读取 AdditionalFiles
   - 配置驱动代码生成
   - 生成特性开关目录
5. `SourceGenerator05-ProjectPractice`
   - 项目实践
   - 为实体生成列名常量
   - 避免 SQL/Dapper 里的魔法字符串
6. `SourceGenerator06-PrincipleAndSourceAnalysis`
   - 原理解析
   - 编译期执行时机
   - 简化版源码分析与职责拆分

## 运行方式

```bash
dotnet build SourceGeneratorNew/SourceGeneratorKeyword.sln

dotnet run --project SourceGeneratorNew/SourceGenerator01-HelloGenerator/HelloGenerator.Consumer
dotnet run --project SourceGeneratorNew/SourceGenerator02-AutoNotify/AutoNotifyGenerator.Consumer
dotnet run --project SourceGeneratorNew/SourceGenerator03-IncrementalEndpoints/EndpointGenerator.Consumer
dotnet run --project SourceGeneratorNew/SourceGenerator04-AdditionalFiles/FeatureFlagGenerator.Consumer
dotnet run --project SourceGeneratorNew/SourceGenerator05-ProjectPractice/ColumnConstantGenerator.Consumer
dotnet run --project SourceGeneratorNew/SourceGenerator06-PrincipleAndSourceAnalysis
```

## 先给结论

- Source Generator 是编译期执行，不是运行时反射。
- 它的本质是：在编译过程中分析用户代码，然后把新的 C# 源码补给编译器。
- 经典接口是 `ISourceGenerator`，推荐路线通常是 `IIncrementalGenerator`。
- 适合场景：重复样板代码、强类型元数据、配置驱动代码生成、编译期约束增强。
- 不适合场景：依赖运行时状态、需要访问数据库/网络、逻辑极重导致编译变慢的任务。

## 设计建议

- 生成器项目和消费项目必须分开看待。
- 生成器关注“发现什么、生成什么”，消费项目关注“如何使用生成结果”。
- 真实项目里，生成器越聚焦越好，不要让它变成新的“大杂烩框架”。
- `IIncrementalGenerator` 优先于传统 `ISourceGenerator`，因为它更适合控制编译性能。
