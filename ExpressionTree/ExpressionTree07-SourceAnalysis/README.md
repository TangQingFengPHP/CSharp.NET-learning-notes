# 07 源码分析

## 分析目标

这里不用真实 ORM 源码大段贴代码，而是用“简化版查询翻译器”理解表达式树在框架里的角色。

## 典型分层

- 服务层：产出表达式树
- 查询编译器：接收表达式树
- Visitor/Translator：把表达式翻译成 SQL-like 条件
- 执行器：交给数据库或其他后端执行

## 要学会观察什么

当你在源码里看到 `ExpressionVisitor`、`LambdaExpression`、`MethodCallExpression` 时，重点问：

- 它在识别什么语义？
- 它准备翻译成什么目标语言？
- 它有没有改写原始树？
