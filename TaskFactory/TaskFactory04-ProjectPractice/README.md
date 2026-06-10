# 04 项目实践：门店订单报表

## 详解

综合练习场景：读取多门店订单 → 并行计算销售额 → 汇总 → 竞速取最快 → 自定义 `TaskFactory` 统一策略。

不依赖数据库，专注 TaskFactory 调度知识点。

## 运行后你会看到什么

1. `StartNew` + `TaskScheduler.Default` 并行计算各门店
2. `ContinueWhenAll` 汇总总销售额
3. `ContinueWhenAny` 取最快门店
4. 自定义 `TaskFactory` 批量创建任务

## 注意点

- 明确指定 `TaskScheduler.Default` 可避免在某些上下文里调度器不符合预期
- 自定义工厂的价值是固定一批任务的默认策略，而非少写几行代码
