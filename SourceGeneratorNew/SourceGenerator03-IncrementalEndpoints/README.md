# 03 Incremental Generator

## 详解

这个示例改用 `IIncrementalGenerator`，演示推荐的现代写法。

它会扫描带 `[GenerateEndpoint]` 的类型，并为每个类型生成：

- `Method`
- `Route`
- `Describe()`

## 为什么要学这个

`IIncrementalGenerator` 相比传统生成器更容易控制性能，因为它可以只对变更相关的输入重新计算。
