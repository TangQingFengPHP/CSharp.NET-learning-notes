# 02 sealed override

## 详解

`sealed override` 只能用于“已经 override 的虚方法/属性/事件”，含义是：

- 当前类可以重写父类行为
- 但当前类的子类不能继续重写这个成员

这适合在继承链中“阶段性开放，最终收口”。

## 语法

```csharp
public class Derived : Base
{
    public sealed override void Execute() { }
}
```

## 应用价值

- 防止某些关键规则被下层子类继续篡改
- 保留上层框架扩展点，同时在具体实现层结束扩展
- 提高阅读者对继承链终点的理解
