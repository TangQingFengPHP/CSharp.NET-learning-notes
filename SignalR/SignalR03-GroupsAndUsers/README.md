# 03 Groups 和 Users

## 详解

SignalR 里最容易混淆的是三个概念：

- `ConnectionId`：一次连接的唯一标识
- `Group`：逻辑分组，一个连接可以在多个组里
- `User`：业务用户标识，通常来自认证系统

## 典型用法

- `Clients.Client(connectionId)`：只发给一个连接
- `Clients.Group(groupName)`：发给一个组
- `Clients.User(userId)`：发给某个用户当前所有连接

## 注意

- `Clients.User(userId)` 依赖 `UserIdentifier`
- 如果没有认证体系，需要自己约定用户映射逻辑
- Group 是内存态概念，分布式部署时要考虑 backplane
