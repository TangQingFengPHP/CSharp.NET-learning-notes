# 02 强类型 Hub

## 详解

默认写法经常是：

```csharp
await Clients.All.SendAsync("ReceiveMessage", value);
```

问题是：

- 事件名是字符串，容易拼错
- 参数个数和顺序不直观
- 重构时缺少编译器保护

强类型 Hub 使用 `Hub<TClient>`：

- 服务端调用客户端方法时有接口约束
- 方法名和参数都有编译期检查
- 更适合中大型项目
