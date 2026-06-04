# EF Core 实战项目（EfCorePractice）

完整 **ASP.NET Core Web API + EF Core 9 + MySQL** 实战，按 .NET 常见分层组织，在真实业务代码中覆盖 EF Core 核心知识点（非按知识点拆 Demo）。

## 技术栈

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9 + Pomelo MySQL Provider
- Swashbuckle（Swagger UI）
- MySQL 8.4 + Docker Compose
- xUnit + WebApplicationFactory（InMemory 冒烟测试）

## 项目结构（Clean Architecture 简化版）

```
EFCore/
├── EfCorePractice.sln
├── EFCore-Guide.md              # 知识点全景对照表
├── src/
│   ├── EfCorePractice.Domain/       # 实体、枚举、领域接口
│   ├── EfCorePractice.Application/  # 应用服务、DTO、Specification
│   ├── EfCorePractice.Infrastructure/ # DbContext、迁移、拦截器、种子
│   └── EfCorePractice.Api/          # API、Swagger、中间件
├── tests/EfCorePractice.Tests/
├── docker-compose.yml
├── docker-compose.dev.yml
└── Dockerfile
```

## 端口

| 服务 | 宿主机 |
| --- | --- |
| MySQL | **3313** |
| API + Swagger | **8185**（`/swagger`） |

## Docker 启动

镜像加速请在 **Docker Desktop** 中自行配置，项目内仅保留标准 `Dockerfile` + `docker-compose.yml`。

```bash
cd EFCore
docker compose up --build -d
```

- Swagger：http://localhost:8185/swagger  
- 健康检查：http://localhost:8185/health  

若本地有 `.env` 且含 `DOTNET_SDK_IMAGE` 等旧变量，请删除这些行或重新 `cp .env.example .env`。

重置数据卷：`docker compose down -v`

仅启动 MySQL（宿主机调试 API）：`docker compose -f docker-compose.yml -f docker-compose.dev.yml up mysql -d`

## 本地开发（不用 API 容器）

```bash
cd EFCore
docker compose -f docker-compose.yml -f docker-compose.dev.yml up mysql -d
dotnet run --project src/EfCorePractice.Api
```

## 数据库迁移（Code First）

```bash
cd EFCore

# 新增迁移
dotnet ef migrations add <MigrationName> \
  --project src/EfCorePractice.Infrastructure \
  --startup-project src/EfCorePractice.Api \
  --output-dir Persistence/Migrations

# 应用到数据库
dotnet ef database update \
  --project src/EfCorePractice.Infrastructure \
  --startup-project src/EfCorePractice.Api

# 查看迁移状态
curl http://localhost:8185/migrations
```

Docker 启动时默认 `ApplyMigrationsOnStartup=true`，自动 `Migrate` 并种子数据。

## 多租户演示

请求头携带租户 ID，全局过滤器只返回该租户用户：

```bash
curl -H "X-Tenant-Id: 1" "http://localhost:8185/users?status=ACTIVE"
curl -H "X-Tenant-Id: 2" "http://localhost:8185/users?status=ACTIVE"
```

## 软删除演示

```bash
# 软删除（Remove 被转换为 IsDeleted=true）
curl -X DELETE "http://localhost:8185/users/4/soft"

# 查看已删除（IgnoreQueryFilters）
curl "http://localhost:8185/users/deleted"

# 恢复
curl -X PUT "http://localhost:8185/users/4/restore"
```

## 三种数据访问模式（并列对照）

| 模式 | 说明 | 入口 |
| --- | --- | --- |
| A | Service 直用 DbContext（**原有全套 API，保留**） | `/users`、`/orders`、`/demo` |
| B | 仓储 + 工作单元 | `/patterns/uow/users` |
| C | 读写分离读库 | `/patterns/read/users` |
| 拦截器 | 慢 SQL 链 + 审计 | `/patterns`、`/patterns/interceptors/*` |

```bash
# 查看三种模式说明
curl http://localhost:8185/patterns

# 模式 B：UoW 创建用户
curl -X POST http://localhost:8185/patterns/uow/users \
  -H "Content-Type: application/json" \
  -d '{"username":"uow","email":"uow@example.com","age":25}'

# 模式 C：读库查询
curl http://localhost:8185/patterns/read/users/1

# 慢 SQL 演示
curl -X POST http://localhost:8185/patterns/interceptors/slow-queries/demo
curl http://localhost:8185/patterns/interceptors/slow-queries
```

## 其他 EF 特性入口（模式 A）

| 能力 | 接口 |
| --- | --- |
| Include / Split Query | `GET /users/{id}/with-orders`、`/with-orders-split` |
| 编译查询 | `GET /demo/compiled/{id}` |
| SQL 计划 | `GET /demo/query-plan` |
| 显式事务 | `POST /demo/transaction` |
| 变更跟踪 | `POST /demo/change-tracking` |
| 原生 SQL | `GET /users/native` |
| 批量更新 | `PUT /users/disable-by-age` |
| 订单 Join DTO | `GET /orders/join` |
| GroupBy 统计 | `GET /orders/stats` |

完整知识点映射见 **[EFCore-Guide.md](./EFCore-Guide.md)**。

## 测试

```bash
dotnet test EfCorePractice.sln
```

## 参考

- 内部笔记：`C#.NET EFCore 详解.md`
- 官方：[What's New in EF Core 9](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-9.0/whatsnew)
- 官方：[EF Core 文档](https://learn.microsoft.com/en-us/ef/core/)
