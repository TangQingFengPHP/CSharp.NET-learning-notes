# 01 基础 Hub

## 详解

SignalR 的核心目的是让服务端可以主动向客户端推送消息，而不是只能被动等 HTTP 请求。

最常见的入口是 `Hub`：

- 客户端连接某个 Hub 地址
- 调用 Hub 方法
- Hub 再通过 `Clients.All` / `Clients.Caller` / `Clients.Others` 等对象回推消息

## 核心对象

- `Hub`：实时通信入口
- `Context.ConnectionId`：当前连接唯一标识
- `Clients`：发送消息给客户端
- `Groups`：将连接加入/移出组

## 适用场景

- 在线聊天
- 实时通知
- 监控面板
- 协作编辑
