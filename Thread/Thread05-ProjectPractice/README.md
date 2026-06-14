# 05 项目实践：专用订单工作线程

## 详解

`BlockingCollection<T>` + 专用 `Thread` 实现长期阻塞式订单处理：入队、顺序消费、单任务异常隔离、停止后 `Join` 收尾。

## 运行后你会看到什么

1. 四个订单依次处理
2. 异常订单 SO003 失败但不影响后续
3. 队列清空后工作线程正常退出

## 注意点

- 专用 Thread 适合同步阻塞循环，异步 I/O 应改用 Channel + BackgroundService
- `CompleteAdding()` + `Join()` 构成明确停止流程
- 不要把 `async` Lambda 直接交给 `Thread`（会变成 `async void`）
