# 01 Hello Generator

## 详解

这是最小可运行的 Source Generator 示例。

结构上分成两部分：

- `HelloGenerator`：生成器项目
- `HelloGenerator.Consumer`：消费项目

它会在编译时生成一个 `Generated.HelloMessages` 类型，消费项目直接调用这个生成出来的方法。

## 要点

- Generator 参与的是编译过程，不是程序启动过程
- Consumer 不需要手写生成后的类
- 生成器输出的是标准 C# 代码，最终仍由编译器统一编译
