# C# Emit 动态生成代码专题

本专题按知识点拆分为独立文件夹，每个文件夹都是一个可直接运行的控制台项目。

`System.Reflection.Emit` 可以在运行时动态生成 IL、方法、类型和程序集，常见于 ORM、Mapper、动态代理等框架底层。

## 目录

1. `Emit01-BasicDynamicMethod`
   - `DynamicMethod` 与 `ILGenerator`
   - 栈式 IL 模型：动态生成加法方法
2. `Emit02-PropertyAccessor`
   - 动态生成属性 Getter / Setter
   - 值类型 `Box` / `Unbox_Any`
3. `Emit03-ObjectMapper`
   - 用 Emit 生成 `User -> UserDto` 映射委托
4. `Emit04-DynamicType`
   - `AssemblyBuilder` / `TypeBuilder` 生成完整类型
5. `Emit05-InterfaceAndCaching`
   - 动态实现接口
   - 委托缓存与反射性能对比

## 运行方式

```bash
dotnet run --project Emit01-BasicDynamicMethod
dotnet run --project Emit02-PropertyAccessor
dotnet run --project Emit03-ObjectMapper
dotnet run --project Emit04-DynamicType
dotnet run --project Emit05-InterfaceAndCaching
```

## 先给结论

- **反射**：运行时读取和调用已有结构
- **Emit**：运行时生成新的 IL 和执行逻辑，生成后缓存委托可接近手写性能
- **只生成单个方法**：优先 `DynamicMethod`
- **生成完整类或接口实现**：`AssemblyBuilder` → `ModuleBuilder` → `TypeBuilder`
- **Emit 生成一次、缓存反复调用**；不要每次调用都重新 Emit

## 技术选型速查

| 场景 | 更合适的选择 |
|------|-------------|
| 编译期就能确定 | Source Generator |
| 运行时拼简单委托 | Expression Tree |
| 运行时生成类型或极致 IL 控制 | Emit |
| 低频动态调用 | 反射 |
| AOT / 裁剪友好 | Source Generator |
