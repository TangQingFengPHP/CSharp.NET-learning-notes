# C# `sealed` 关键词专题

本专题按知识点拆分为独立文件夹，每个文件夹都是一个可直接运行的控制台项目。

## 目录

1. `Sealed01-BasicClass`
   - `sealed` 修饰类的基本语义
   - 为什么不能被继承
   - 与 `struct` 的关系
2. `Sealed02-SealedOverride`
   - `sealed override` 的用法
   - 为什么它只能用于重写后的虚方法
   - 多态链路如何被截断
3. `Sealed03-ApplicationScenarios`
   - 典型应用场景
   - API 稳定性、安全边界、值对象封装
4. `Sealed04-ProjectPractice`
   - 项目实践示例
   - 订单状态流转、领域服务、防止错误扩展
5. `Sealed05-PrincipleAndIL`
   - 编译器视角
   - CLR/IL 层面的理解
   - 虚调用与去虚拟化思路
6. `Sealed06-SourceAnalysis`
   - 框架设计里的 `sealed` 思路
   - 简化版“源码分析”
   - 如何在自己项目中套用这些模式

## 运行方式

```bash
dotnet run --project Sealed01-BasicClass
dotnet run --project Sealed02-SealedOverride
dotnet run --project Sealed03-ApplicationScenarios
dotnet run --project Sealed04-ProjectPractice
dotnet run --project Sealed05-PrincipleAndIL
dotnet run --project Sealed06-SourceAnalysis
```

## 先给结论

- `sealed class`：禁止其他类继承当前类。
- `sealed override`：允许当前类重写父类虚方法，但禁止再往下继续重写。
- `struct` 天生不可继承，因此不需要再写 `sealed struct`。
- `sealed` 的核心目的不是“省性能”，而是“固定扩展边界”；性能收益通常只是附带结果。
- 在项目中适合用于：值对象、工具类、最终实现类、领域规则不允许二次扩展的组件。

## 设计建议

- 默认不要滥用 `sealed`，否则会降低可测试性和扩展性。
- 如果某个类型的继承语义本身就不成立，应优先显式 `sealed`。
- 如果父类开放了虚方法，但某一层实现已经是最终版本，可用 `sealed override` 收口。
- 设计公共库时，是否允许继承，应该是 API 设计的一部分，而不是后期随意修改。
