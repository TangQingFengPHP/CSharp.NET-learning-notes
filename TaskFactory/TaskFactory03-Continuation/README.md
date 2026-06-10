# 03 ContinueWhenAll / ContinueWhenAny

## 详解

- `ContinueWhenAll`：一组任务全部完成后执行延续
- `ContinueWhenAny`：任意一个任务完成后执行延续

现代代码也可用 `Task.WhenAll` / `Task.WhenAny`，通常更贴近 `async/await` 风格。

## 运行后你会看到什么

1. 三门店并行计算后汇总总销售额
2. 不同延迟的门店任务，取最快返回
3. `Task.WhenAll` 的现代写法对比

## 注意点

- 延续任务里访问 `task.Result` 时，对应任务必须已完成
- 新代码优先考虑 `WhenAll` / `WhenAny`，需要延续调度控制时用 `ContinueWhen*`
