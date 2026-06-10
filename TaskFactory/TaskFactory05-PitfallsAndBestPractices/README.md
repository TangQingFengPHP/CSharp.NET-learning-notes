# 05 常见坑与最佳实践

## 详解

| 坑 | 处理方式 |
|----|---------|
| `StartNew` + `async` 委托 | 产生 `Task<Task<T>>`，用 `Unwrap()` 或改 `Task.Run` |
| `Result` / `Wait()` | 阻塞线程，异常包在 `AggregateException` |
| 父子任务 | `AttachedToParent`；`Task.Run` 默认 `DenyChildAttach` |
| 旧式 APM | `Task.Factory.FromAsync` |

## 运行后你会看到什么

1. async 委托嵌套 Task 与 Unwrap 对比
2. await vs Result 的异常差异
3. AttachedToParent 父子任务完成顺序
4. FromAsync 读取文件示例

## 注意点

- 异步 I/O 直接 `await`，不要用 `StartNew` 包一层
- 异步代码里用 `await`，不要用 `Wait()` / `Result`
