# 01 基础概念

## 详解

表达式树是 `System.Linq.Expressions` 命名空间中的一套对象模型，用来表示代码结构。

最常见的写法：

```csharp
Expression<Func<int, int>> expr = x => x + 1;
```

这不是直接把 lambda 编译成委托，而是生成一棵可分析的树。

## 核心区别

- `Func<int, int>`：只能执行
- `Expression<Func<int, int>>`：可以执行，也可以分析、改写、翻译

## 常见节点

- `Parameter`
- `Constant`
- `MemberAccess`
- `Call`
- `Equal`
- `GreaterThan`
- `AndAlso`
