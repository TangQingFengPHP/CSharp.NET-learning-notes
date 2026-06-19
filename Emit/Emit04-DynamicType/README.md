# 04 动态生成完整类型

## 详解

用 `AssemblyBuilder` / `TypeBuilder` 运行时生成带字段、构造函数、属性和方法的完整类。

## 运行后你会看到什么

1. 动态创建 `RuntimePerson("赵六")`
2. 通过反射设置 `Name`，调用 `SayHello()`

## 注意点

- 属性需要分别 Define get/set 方法并 `SetGetMethod` / `SetSetMethod`
- `AssemblyBuilderAccess.RunAndCollect` 允许 GC 回收动态程序集
