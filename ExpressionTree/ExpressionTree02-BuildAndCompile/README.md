# 02 手动构建与编译

## 详解

表达式树不仅能由编译器生成，也能手动拼出来。

这是理解表达式树最直接的方法，因为你会看到：

- 参数节点怎么声明
- 常量节点怎么放进去
- 二元运算怎么组合
- 最后如何包装成 Lambda

## 核心 API

- `Expression.Parameter`
- `Expression.Constant`
- `Expression.Add`
- `Expression.Multiply`
- `Expression.Lambda`
- `Compile()`
