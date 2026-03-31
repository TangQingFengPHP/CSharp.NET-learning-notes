# 04 AdditionalFiles

## 详解

这个示例演示配置驱动代码生成。

消费项目提供一个 `feature-flags.txt` 文件作为 AdditionalFiles，生成器读取它并输出：

- `Generated.FeatureFlags` 静态类
- `All` 集合
- 每个特性开关的强类型常量

## 适用场景

- 特性开关
- 权限点目录
- 路由清单
- 配置模板生成
