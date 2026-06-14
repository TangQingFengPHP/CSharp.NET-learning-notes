# 03 协作取消与异常处理

## 详解

现代 .NET 不推荐 `Thread.Abort()`。应通过 `CancellationToken` 发送取消信号，线程主动退出。`Interrupt()` 只中断处于 Sleep/Wait/Join 的线程。

## 运行后你会看到什么

1. 订单轮询线程通过 Token 协作取消
2. `Interrupt` 中断无限 Sleep
3. 线程入口捕获异常并传回主线程

## 注意点

- `token.WaitHandle.WaitOne` 比 `Thread.Sleep` 更能及时响应取消
- 未捕获的线程异常可能终止整个进程
