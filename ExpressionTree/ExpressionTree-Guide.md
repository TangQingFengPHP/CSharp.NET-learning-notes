# C# .NET Expression Tree 表达式树专题

本专题沿用之前的学习目录风格：一个知识点一个文件夹，每个文件夹都是独立可运行项目，并附一份中文说明。

## 目录

1. `ExpressionTree01-Basics`
   - 表达式树是什么
   - `Func<T>` 与 `Expression<Func<T>>` 的区别
   - 常见节点类型
2. `ExpressionTree02-BuildAndCompile`
   - 手动构建表达式树
   - `ParameterExpression`、`BinaryExpression`、`LambdaExpression`
   - `Compile()` 的作用
3. `ExpressionTree03-VisitorAndParse`
   - `ExpressionVisitor` 的用法
   - 如何遍历和改写表达式树
   - 节点解析思路
4. `ExpressionTree04-DynamicPredicateBuilder`
   - 动态拼接条件
   - `AndAlso` / `OrElse`
   - 动态查询过滤器构建
5. `ExpressionTree05-ProjectPractice`
   - 项目实践
   - Repository / Query 场景
   - 用户筛选、搜索条件拼装
6. `ExpressionTree06-PrincipleAndExecution`
   - 原理解析
   - 委托执行 vs 表达式树编译
   - 为什么 ORM 喜欢表达式树
7. `ExpressionTree07-SourceAnalysis`
   - 简化版源码分析
   - 查询翻译器、访问器、执行器的职责拆分
   - 从表达式树到 SQL-like 语句的思路
8. `ExpressionTree08-MinimalApiQueryDemo`
   - Minimal API 查询接口
   - 前端筛选条件 -> 动态表达式树
   - 浏览器查看表达式字符串和结果

## 运行方式

```bash
dotnet run --project ExpressionTree/ExpressionTree01-Basics
dotnet run --project ExpressionTree/ExpressionTree02-BuildAndCompile
dotnet run --project ExpressionTree/ExpressionTree03-VisitorAndParse
dotnet run --project ExpressionTree/ExpressionTree04-DynamicPredicateBuilder
dotnet run --project ExpressionTree/ExpressionTree05-ProjectPractice
dotnet run --project ExpressionTree/ExpressionTree06-PrincipleAndExecution
dotnet run --project ExpressionTree/ExpressionTree07-SourceAnalysis
dotnet run --project ExpressionTree/ExpressionTree08-MinimalApiQueryDemo
```

## 先给结论

- 表达式树本质上是“代码的抽象语法树对象化结果”。
- `Func<T>` 只能执行，`Expression<Func<T>>` 可以被分析、改写、翻译。
- ORM、动态规则引擎、查询构建器很喜欢表达式树，因为它们需要“看懂你的代码”。
- 表达式树不是为了替代普通 lambda，而是为了在运行时拿到“可操作的代码结构”。
- 真正的难点不在 `Compile()`，而在“如何解析、组合、翻译”。

## 使用建议

- 只需要执行时，优先用普通委托。
- 需要做动态查询、规则组合、翻译成 SQL/DSL 时，再用表达式树。
- 写动态条件时，注意参数复用，不要随便把不同参数直接拼到一棵树里。
- `ExpressionVisitor` 是表达式树实战里非常重要的入口。
- 涉及 Web 查询接口时，推荐把“接收查询参数”和“拼表达式树”拆到不同层级，避免 API 端点变成一坨条件判断。
