# 02 动态属性 Getter / Setter

## 详解

根据类型和属性名，运行时生成类似 `((User)instance).Name` 的访问逻辑，替代高频 `PropertyInfo.GetValue/SetValue`。

## 运行后你会看到什么

1. 动态 Getter 读取 `Name`、`Age`
2. 动态 Setter 写入后再次读取

## 注意点

- Getter 返回 `object` 时，值类型需 `Box`
- Setter 接收 `object` 写入值类型时，需 `Unbox_Any`
- `skipVisibility: true` 可访问非 public 成员
