# 05 动态接口实现与委托缓存

## 详解

运行时生成 `ICalculator` 实现类，并用 `ConcurrentDictionary` 缓存 Getter 委托。这是动态代理、Mock 框架的简化版基础。

## 运行后你会看到什么

1. 动态 `ICalculator.Add(3, 5)` 输出日志并返回 8
2. 同一属性的 Getter 委托只生成一次
3. 100 万次反射 vs Emit 粗测对比

## 注意点

- 接口实现的实例方法：`Ldarg_0` = this，`Ldarg_1`/`Ldarg_2` = 业务参数
- `DefineMethodOverride` 绑定接口方法
- Emit 适合生成一次、反复调用
