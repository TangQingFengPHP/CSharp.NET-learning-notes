# C# .NET SignalR 专题

本专题沿用 `SealedNew` 的组织方式：一个知识点一个文件夹，每个文件夹都是独立可运行项目，并附一份中文说明。

## 目录

1. `SignalR01-BasicHub`
   - SignalR 是什么
   - `Hub` 的基本概念
   - 广播、连接、调用的最小示例
2. `SignalR02-StronglyTypedHub`
   - 强类型 Hub
   - `Hub<TClient>` 的价值
   - 服务端推送如何避免魔法字符串
3. `SignalR03-GroupsAndUsers`
   - Group、User、ConnectionId 的关系
   - 按组推送、定向推送
   - 典型聊天室/项目房间场景
4. `SignalR04-ProjectPractice`
   - 项目实践
   - 后台任务结合 `IHubContext`
   - 订单/通知中心/实时看板模式
5. `SignalR05-PrincipleAndPipeline`
   - SignalR 调用链路
   - `Hub` 生命周期
   - `IHubFilter`、连接建立、方法拦截
6. `SignalR06-SourceAnalysis`
   - 源码设计思路
   - `HubLifetimeManager`、`HubContext`、调度器的职责拆分
   - 为什么 SignalR 适合“抽象 + 默认实现”架构
7. `SignalR07-BrowserDemo`
   - 浏览器 HTML 联调 demo
   - 前后端同项目启动
   - 页面可直接连接、收发消息、看在线效果

## 运行方式

```bash
dotnet run --project SignalR/SignalR01-BasicHub
dotnet run --project SignalR/SignalR02-StronglyTypedHub
dotnet run --project SignalR/SignalR03-GroupsAndUsers
dotnet run --project SignalR/SignalR04-ProjectPractice
dotnet run --project SignalR/SignalR05-PrincipleAndPipeline
dotnet run --project SignalR/SignalR06-SourceAnalysis
dotnet run --project SignalR/SignalR07-BrowserDemo
```

## 先给结论

- SignalR 是 ASP.NET Core 的实时通信框架，适合服务端主动推送。
- 常见入口是 `Hub`，客户端连接后可以双向调用。
- `Clients.All`、`Clients.Group`、`Clients.User` 分别对应广播、分组推送、定向推送。
- 生产环境里更常见的做法是：业务服务通过 `IHubContext` 推送，而不是把业务逻辑全部塞进 `Hub`。
- `Hub` 是“通信入口”，不应该演变成“大而全的业务服务类”。

## 使用建议

- `Hub` 负责通信协议和连接上下文，业务逻辑放到服务层。
- 能用强类型 Hub 就尽量不用字符串事件名。
- 需要跨实例广播时，要考虑 Redis/Azure SignalR 等 backplane 方案。
- 对 `UserIdentifier`、鉴权、组管理、断线重连要提前设计，不要等线上补救。
- 浏览器调试时，如果前端用 CDN 引入 SignalR JS，请确保浏览器可访问外网 CDN。
