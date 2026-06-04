using EfCorePractice.Domain.Entities;
using EfCorePractice.Domain.Enums;
using EfCorePractice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCorePractice.Infrastructure.Seed;

public class DatabaseSeeder(AppDbContext db, ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await db.Users.IgnoreQueryFilters().AnyAsync(ct))
        {
            return;
        }

        logger.LogInformation("正在写入种子数据...");

        var users = new[]
        {
            new User
            {
                Username = "张三",
                Email = "zhangsan@example.com",
                Age = 20,
                Status = "ACTIVE",
                TenantId = 1,
                Contact = new UserContactProfile
                {
                    Email = "zhangsan@example.com",
                    Phone = "13800000001",
                    Tags = ["vip", "north"]
                }
            },
            new User
            {
                Username = "李四",
                Email = "lisi@example.com",
                Age = 25,
                Status = "ACTIVE",
                TenantId = 1,
                Contact = new UserContactProfile
                {
                    Email = "lisi@example.com",
                    Phone = "13800000002",
                    Tags = ["member"]
                }
            },
            new User
            {
                Username = "王五",
                Email = "wangwu@example.com",
                Age = 17,
                Status = "DISABLED",
                TenantId = 2,
                Contact = new UserContactProfile
                {
                    Email = "wangwu@example.com",
                    Phone = "13800000003",
                    Tags = []
                }
            }
        };

        db.Users.AddRange(users);
        await db.SaveChangesAsync(ct);

        db.Orders.AddRange(
            new Order
            {
                UserId = users[0].Id,
                OrderNo = "A001",
                Amount = 99.00m,
                Status = OrderStatus.Paid,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new Order
            {
                UserId = users[0].Id,
                OrderNo = "A002",
                Amount = 260.00m,
                Status = OrderStatus.Paid,
                CreatedAt = DateTime.UtcNow.AddDays(-28)
            },
            new Order
            {
                UserId = users[1].Id,
                OrderNo = "A003",
                Amount = 35.50m,
                Status = OrderStatus.Cancelled,
                CreatedAt = DateTime.UtcNow.AddDays(-27)
            });

        await db.SaveChangesAsync(ct);
        logger.LogInformation("种子数据写入完成");
    }
}
