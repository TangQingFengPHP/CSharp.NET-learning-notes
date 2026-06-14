# 04 共享数据同步

## 详解

`count++` 包含读-改-写三步，多线程会互相覆盖。简单原子操作用 `Interlocked`，复合逻辑用 `lock`。

## 运行后你会看到什么

1. 无同步时 20 万次自增结果偏小
2. `Interlocked.Increment` 得到正确 200000
3. `lock` 保护银行账户提现

## 注意点

- `volatile` 不能保证复合操作原子性
- 锁对象应私有专用，不要 `lock(this)` 或 `lock(typeof(...))`
