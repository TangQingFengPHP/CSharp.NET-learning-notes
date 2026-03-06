# 01 基础概念

## 详解

`sealed` 修饰类时，表示该类是最终类型，不能再作为基类被继承。

```csharp
public sealed class TokenProvider
{
}
```

如果再写：

```csharp
public class MyTokenProvider : TokenProvider
{
}
```

编译器会直接报错。

## 为什么需要它

- 防止错误扩展
- 固定对象行为边界
- 减少对继承链的维护成本
- 明确表达“这是最终实现”

## 注意点

- `sealed` 不能修饰抽象类，因为 `abstract` 要求可被继承，而 `sealed` 禁止继承，二者语义冲突。
- `struct` 本身就是隐式 sealed，因为结构体不能被继承。
- `sealed` 不影响实例化，只影响继承。
