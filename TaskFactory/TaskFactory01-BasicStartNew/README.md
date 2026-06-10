# 01 StartNew 基础与 Task.Run 对比

## 详解

`Task.Factory.StartNew` 创建并启动任务，比 `Task.Run` 参数更多，也更容易写错。

`Task.Run` 大致等价于：

```csharp
Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
```

## 运行后你会看到什么

1. 无返回值的基础 `StartNew`
2. 返回 `Task<StoreReport>` 的带结果任务
3. `Task.Run` 与显式配置的 `StartNew` 对比
4. 通过 `state` 参数传递门店数据，避免闭包

## 注意点

- 简单后台任务优先 `Task.Run`
- 需要指定调度器或创建选项时再考虑 `StartNew`
