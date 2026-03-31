# 02 AutoNotify

## 详解

这个示例用 Attribute 驱动生成属性代码。

开发者只写字段：

- `[AutoNotify] private string _name;`

生成器会自动生成：

- `public string Name { get; set; }`

## 价值

- 减少 ViewModel / DTO / Settings 类里的重复样板代码
- 演示如何从语法节点进入语义分析
- 演示源生成器里非常常见的“标记属性 + 部分类型生成”模式
