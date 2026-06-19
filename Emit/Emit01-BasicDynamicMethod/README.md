# 01 DynamicMethod 与 IL 栈模型

## 详解

`DynamicMethod` 在内存中创建轻量级动态方法，通过 `ILGenerator` 逐条发出 IL 指令，最后用 `CreateDelegate` 转成委托调用。

生成 `int Add(int a, int b)` 的 IL 思路：

```text
Ldarg_0  // 压入 a
Ldarg_1  // 压入 b
Add      // 弹出两个值相加
Ret      // 返回栈顶
```

## 运行后你会看到什么

1. 动态生成的 `add(10, 20)` 返回 30
2. 等价 IL 指令说明

## 注意点

- 静态方法参数从 `Ldarg_0` 开始
- `Ret` 前栈顶必须与返回类型匹配
