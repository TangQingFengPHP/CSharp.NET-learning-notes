# 02 取消令牌与 LongRunning

## 详解

`CancellationToken` 不会强制杀死线程，任务内部需主动调用 `ThrowIfCancellationRequested()`。

`TaskCreationOptions.LongRunning` 是给调度器的提示，适合长时间阻塞的后台循环，不适合包装 `async` I/O。

## 运行后你会看到什么

1. 处理订单批次时被外部取消
2. `LongRunning` 任务在独立线程上轮询后台队列

## 注意点

- 只传 Token 但内部不检查，正在执行的代码不会自动停下
- 不要把 `LongRunning` 当成性能优化开关
