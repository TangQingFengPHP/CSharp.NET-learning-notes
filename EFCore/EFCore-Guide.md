# EF Core 知识点全景（实战对照表）

本文档结合 [C#.NET EFCore 详解](file:///Users/panfeng/projects/MyNote/dotnet通关秘籍/C#.NET%20EFCore%20详解.md)、[Microsoft EF Core 9 新特性](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-9.0/whatsnew) 与本仓库 `EfCorePractice` 项目，按 **领域 → 持久化 → 查询 → 高级 → 运维** 梳理。

## 一、基础与建模

| 知识点 | 说明 | 本项目位置 |
| --- | --- | --- |
| DbContext / DbSet | ORM 入口与表映射 | `Infrastructure/Persistence/AppDbContext.cs` |
| 实体 POCO | 领域模型 | `Domain/Entities/` |
| 数据注解 | `[Required]`、`[MaxLength]`、`[Table]` | `Domain/Entities/User.cs`、`Order.cs` |
| Fluent API | 表名、索引、关系、列类型 | `AppDbContext.OnModelCreating` |
| 一对多 / 多对一 | `User.Orders` / `Order.User` | 同上 |
| 值转换器 | 枚举↔字符串、对象↔JSON 字符串 | `Order.Status`、`User.Contact` |
| `[NotMapped]` | 不持久化计算属性 | `User.DisplayName` |
| Owned 类型概念 | 复杂对象嵌入（JSON 列实现） | `UserContactProfile` + JSON 转换 |
| 并发令牌 | `Version` + `IsConcurrencyToken` | `User.Version` |
| 审计字段 | 创建/更新时间 | `AuditableEntityInterceptor` |
| 数据库迁移 | Code First Migrations | `Persistence/Migrations/` |
| 设计时工厂 | `dotnet ef` 工具创建上下文 | `DesignTimeDbContextFactory.cs` |
| 种子数据 | 启动后写入演示数据 | `Seed/DatabaseSeeder.cs` |

## 二、CRUD 与变更跟踪

| 知识点 | 说明 | API / 代码 |
| --- | --- | --- |
| Add / SaveChanges | 插入 | `POST /users` |
| 查询 | LINQ | 各 `GET /users/*` |
| 变更跟踪更新 | 加载实体后改属性 | `PUT /users/{id}/email` |
| Attach / State | 跟踪状态演示 | `POST /demo/change-tracking` |
| Remove 硬删 | `ExecuteDeleteAsync` | `DELETE /users/{id}` |
| 软删除 | 过滤器 + Delete→Update | `DELETE /users/{id}/soft` |
| 恢复软删 | `IgnoreQueryFilters` + Update | `PUT /users/{id}/restore` |
| 批量更新 | `ExecuteUpdateAsync` | `PUT /users/{id}/disable`、`disable-by-age` |
| 乐观并发 | `DbUpdateConcurrencyException` | `PUT /users/{id}/optimistic-disable` |
| 显式事务 | `BeginTransactionAsync` | `POST /demo/transaction` |

## 三、查询进阶

| 知识点 | 说明 | API / 代码 |
| --- | --- | --- |
| Include 预加载 | 避免 N+1 | `GET /users/{id}/with-orders` |
| AsSplitQuery | 拆分查询防笛卡尔积 | `GET /users/{id}/with-orders-split` |
| AsNoTracking | 只读查询 | `UserService` 各查询方法 |
| Select 投影 | DTO 直查 | `GET /users/summary` |
| 动态条件 | 类 Specification | `Specifications/UserSpecifications.cs` |
| 分页 Page | Count + Skip/Take | `GET /users`、`POST /users/search` |
| Slice | 不查总数 | `GET /users/slice` |
| 原生 SQL | `FromSqlInterpolated` | `GET /users/native` |
| Join 投影 | 多表 Select DTO | `GET /orders/join` |
| GroupBy 聚合 | 按状态统计 | `GET /orders/stats` |
| 编译查询 | `EF.CompileAsyncQuery` | `GET /demo/compiled/{id}` |
| ToQueryString | 查看生成 SQL | `GET /demo/query-plan` |

## 四、全局能力

| 知识点 | 说明 | 本项目位置 |
| --- | --- | --- |
| 全局查询过滤器 | `HasQueryFilter` | 软删除 + 多租户 |
| IgnoreQueryFilters | 忽略过滤器 | `GET /users/deleted` |
| 多租户 | Header `X-Tenant-Id` | `Tenancy/HttpTenantContext.cs` |
| 插入时写租户 | SaveChanges 钩子 | `AppDbContext.ApplyTenantOnInsert` |
| SaveChanges 拦截器 | 审计 | `AuditableEntityInterceptor` |
| 连接弹性 | `EnableRetryOnFailure` | `DependencyInjection.cs` |
| 依赖注入 | Scoped DbContext | `AddInfrastructure` |
| 启动迁移 | `Database.MigrateAsync` | `ApplyInfrastructureAsync` |

## 五、EF Core 9 相关（MySQL 场景可落地的部分）

| 特性 | 说明 | 本项目 |
| --- | --- | --- |
| ExecuteUpdate/Delete 增强 | 批量写库 | 用户禁用、删除 |
| LINQ 翻译改进 | 更优 SQL | 订单统计 GroupBy |
| 改进的种子/迁移工具链 | Design + CLI | README 迁移命令 |
| FromSql 更安全参数化 | 参数绑定 | `FromSqlInterpolated` |
| Cosmos/NativeAOT 等 | 本模块使用 **MySQL**，未演示 | 文档标注 |

## 六、工具与运维

```bash
# 添加迁移
dotnet ef migrations add <Name> \
  --project src/EfCorePractice.Infrastructure \
  --startup-project src/EfCorePractice.Api \
  --output-dir Persistence/Migrations

# 应用迁移
dotnet ef database update \
  --project src/EfCorePractice.Infrastructure \
  --startup-project src/EfCorePractice.Api

# 生成 SQL 脚本
dotnet ef migrations script \
  --project src/EfCorePractice.Infrastructure \
  --startup-project src/EfCorePractice.Api -o migrate.sql
```

## 七、架构模式对照（并列保留，互不覆盖）

| 模式 | 说明 | 路由前缀 | DbContext / 组件 |
| --- | --- | --- | --- |
| **A** Service + DbContext | 原有实战入口，功能最全 | `/users`、`/orders`、`/demo` | `AppDbContext` + `UserService` |
| **B** Repository + UnitOfWork | 仓储封装 + 统一 Commit | `/patterns/uow/users` | `IUnitOfWork`、`UserRepository` |
| **C** 读写分离 | 读库默认 NoTracking | `/patterns/read/users`（读） | `AppReadDbContext` / 写仍用 A |
| **拦截器链** | 慢 SQL + 审计 | `/patterns/interceptors/*` | `SlowQueryInterceptor` → `AuditableEntityInterceptor` |

### 模式 A（保留）

- 全部 EF 知识点的主入口：迁移、软删、多租户、ExecuteUpdate、事务（ExecutionStrategy）等。
- 代码：`Application/Services/UserService.cs`、`Api/Controllers/UsersController.cs`

### 模式 B（新增）

- `IUserRepository` / `IOrderRepository` / `IUnitOfWork`
- `POST /patterns/uow/users/with-order`：同一 Scoped 内两次 `CommitAsync` 演示 UoW
- 代码：`Infrastructure/Repositories/`、`UserUnitOfWorkService.cs`

### 模式 C（新增）

- `AppReadDbContext`：与写库共享 `AppDbContextModelConfiguration`，DI 注册 `UseQueryTrackingBehavior(NoTracking)`
- 只读 API，写操作请走模式 A 或 B
- 代码：`UserReadService.cs`、`ReadUsersController.cs`

### 拦截器链（新增）

| 顺序 | 拦截器 | 类型 | 职责 |
| --- | --- | --- | --- |
| 1 | `SlowQueryInterceptor` | `DbCommandInterceptor` | 耗时 ≥ 阈值写日志 + `SlowQueryMetrics` |
| 2 | `AuditableEntityInterceptor` | `SaveChangesInterceptor` | `CreatedAt` / `UpdatedAt` |

- `GET /patterns`：三种模式说明
- `GET /patterns/interceptors/slow-queries`：最近慢 SQL
- `POST /patterns/interceptors/slow-queries/demo`：MySQL `SLEEP` 触发慢查询
- 配置：`appsettings.json` → `SlowQuery:ThresholdMs`

## 八、推荐学习顺序

1. `GET /patterns` 了解三种模式路由  
2. 模式 A：`POST /users` → `GET /users/{id}/with-orders`  
3. 模式 B：对照 `POST /patterns/uow/users` 与 A 的请求体  
4. 模式 C：`GET /patterns/read/users/{id}`，观察读库 NoTracking  
5. `POST /patterns/interceptors/slow-queries/demo` 后查看慢 SQL 列表  
6. 软删除、租户、`/migrations`、`dotnet ef` 迁移  
