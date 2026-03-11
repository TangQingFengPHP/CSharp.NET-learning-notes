# 08 Expression Tree + Minimal API 查询接口

## 目标

这个示例把表达式树放到一个真实的 Web 场景里：

- 前端页面输入筛选条件
- Minimal API 接收查询参数
- 服务端动态构建 `Expression<Func<Product, bool>>`
- 用表达式树过滤数据
- 把“生成的表达式字符串”和结果一起返回

## 为什么这个示例有价值

前面几个项目主要解决“表达式树是什么、怎么构建、怎么解析”。

这个项目解决的是：

- 在 Web API 里怎么接收筛选参数
- 在服务层怎么把筛选参数翻译成表达式树
- 在浏览器里怎么直观看到表达式树最终长什么样

## 访问方式

启动后打开：

- `/`
- API: `/api/products/search`

## 查询参数

- `keyword`
- `category`
- `minPrice`
- `maxPrice`
- `onlyInStock`
