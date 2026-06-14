# C# Thread 线程专题

本专题按知识点拆分为独立文件夹，每个文件夹都是一个可直接运行的控制台项目。

`Thread` 表示一条独立执行路径，可直接控制启动、名称、前后台属性、优先级和等待。现代 .NET 中，普通并发优先 `Task` / `async`，只有需要专用线程时才用 `Thread`。

## 目录

1. `Thread01-BasicCreation`
   - 创建与启动线程、`Join` 等待
   - Lambda 传参、`ParameterizedThreadStart`、通过共享变量返回结果
2. `Thread02-BackgroundAndJoin`
   - 前台线程与后台线程
   - `Join` 超时、`Thread.Sleep` vs `Task.Delay`
3. `Thread03-CancellationAndException`
   - `CancellationToken` 协作取消
   - `Interrupt` 中断等待、线程异常捕获
4. `Thread04-Synchronization`
   - 共享变量竞态、`Interlocked`、`lock`
5. `Thread05-ProjectPractice`
   - `BlockingCollection` + 专用订单工作线程

## 运行方式

```bash
dotnet run --project Thread01-BasicCreation
dotnet run --project Thread02-BackgroundAndJoin
dotnet run --project Thread03-CancellationAndException
dotnet run --project Thread04-Synchronization
dotnet run --project Thread05-ProjectPractice
```

## 先给结论

- **异步 I/O**：`async/await`
- **普通后台计算**：`Task.Run`
- **长期阻塞专用循环**：`Thread`
- **Thread 无返回值**：通过共享变量 + `Join` 同步，或改用 `Task<T>`
- **停止线程**：协作取消 + `Join`，不要用 `Abort`

## 选型速查

| 场景 | 推荐 |
|------|------|
| HTTP / 数据库 / 文件 I/O | `async/await` |
| 短期 CPU 计算、需要返回值 | `Task.Run` |
| 长期专用阻塞循环 | `Thread` |
| 大量短小任务 | `ThreadPool` / `Task` |
| 共享状态修改 | `lock` / `Interlocked` |
