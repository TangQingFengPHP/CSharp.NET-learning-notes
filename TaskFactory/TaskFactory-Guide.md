# C# TaskFactory 任务调度专题

本专题按知识点拆分为独立文件夹，每个文件夹都是一个可直接运行的控制台项目。

`TaskFactory` 适合需要精细控制任务创建、调度器、取消令牌、创建选项和延续任务的场景。普通后台任务用 `Task.Run` 通常更清楚。

## 目录

1. `TaskFactory01-BasicStartNew`
   - `Task.Factory.StartNew` 基本用法
   - 与 `Task.Run` 的对比
   - 通过 `state` 参数减少闭包
2. `TaskFactory02-CancellationAndLongRunning`
   - `CancellationToken` 取消任务
   - `TaskCreationOptions.LongRunning`
3. `TaskFactory03-Continuation`
   - `ContinueWhenAll` 全部完成后汇总
   - `ContinueWhenAny` 取最快返回结果
4. `TaskFactory04-ProjectPractice`
   - 门店订单报表综合练习
   - 并行计算、汇总、竞速、自定义 `TaskFactory`
5. `TaskFactory05-PitfallsAndBestPractices`
   - `async` 委托嵌套 `Task` 与 `Unwrap`
   - 异常处理、`AttachedToParent`、`FromAsync`

## 运行方式

```bash
dotnet run --project TaskFactory01-BasicStartNew
dotnet run --project TaskFactory02-CancellationAndLongRunning
dotnet run --project TaskFactory03-Continuation
dotnet run --project TaskFactory04-ProjectPractice
dotnet run --project TaskFactory05-PitfallsAndBestPractices
```

## 先给结论

- **普通后台计算**：优先 `Task.Run`
- **需要 LongRunning / 自定义调度器 / 延续选项**：用 `TaskFactory`
- **等待多个任务**：现代代码优先 `Task.WhenAll` / `Task.WhenAny`，`ContinueWhenAll` / `ContinueWhenAny` 适合需要延续调度控制的场景
- **async 委托 + StartNew**：会产生 `Task<Task<T>>`，需 `Unwrap()` 或改用 `Task.Run`
- **取消**：`CancellationToken` 只是信号，任务内部要主动检查

## 选型速查

| 场景 | 推荐 |
|------|------|
| 普通后台计算 | `Task.Run` |
| 长时间阻塞 / 后台轮询 | `StartNew` + `LongRunning` |
| 指定 `TaskScheduler` | `TaskFactory` |
| 包装旧式 Begin/End API | `Task.Factory.FromAsync` |
| 父子任务等待 | `AttachedToParent` |
