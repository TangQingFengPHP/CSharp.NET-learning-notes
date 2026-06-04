using EfCorePractice.Application.Abstractions;
using EfCorePractice.Application.Models;
using EfCorePractice.Domain.Entities;
using EfCorePractice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EfCorePractice.Application.Services;

public class DemoService(IApplicationDbContext db, ICompiledQueryService compiledQueries)
{
    public QueryPlanDto GetUserQueryPlan(int minAge) =>
        new(
            "Users.Where(u => u.Age >= minAge).OrderByDescending(u => u.Id)",
            db.Users.Where(u => u.Age >= minAge).OrderByDescending(u => u.Id).ToQueryString());

    public Task<User?> GetUserCompiledAsync(long id, CancellationToken ct = default) =>
        compiledQueries.GetUserByIdAsync(id, ct);

    public Task<TransactionDemoResult> CreateUserWithOrderInTransactionAsync(
        CreateUserWithOrderRequest request,
        CancellationToken ct = default)
    {
        // 启用 EnableRetryOnFailure 后，显式事务必须包在 ExecutionStrategy 中执行
        var strategy = db.Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(ct);

            try
            {
                var user = new User
                {
                    Username = request.User.Username,
                    Email = request.User.Email,
                    Age = request.User.Age,
                    Status = "ACTIVE",
                    TenantId = 1,
                    Contact = request.User.Contact ?? new UserContactProfile { Email = request.User.Email }
                };

                db.Users.Add(user);
                await db.SaveChangesAsync(ct);

                var order = new Order
                {
                    UserId = user.Id,
                    OrderNo = request.OrderNo,
                    Amount = request.Amount,
                    Status = OrderStatus.Paid,
                    CreatedAt = DateTime.UtcNow
                };

                db.Orders.Add(order);
                await db.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                return new TransactionDemoResult(user.Id, order.Id, "用户与订单在同一事务中提交成功");
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }

    public async Task<ChangeTrackingDemoDto> DemonstrateChangeTrackingAsync(CancellationToken ct = default)
    {
        if (db is not DbContext context)
        {
            throw new InvalidOperationException("需要 DbContext 实例演示变更跟踪");
        }

        var detached = new User
        {
            Username = "跟踪演示",
            Email = $"track-{Guid.NewGuid():N}@example.com",
            Age = 30,
            Status = "ACTIVE",
            TenantId = 1
        };

        var initial = context.Entry(detached).State.ToString();

        context.Attach(detached);
        detached.Username = "跟踪演示-已修改";
        var afterAttach = context.Entry(detached).State.ToString();

        context.Add(detached);
        await context.SaveChangesAsync(ct);
        var afterSave = context.Entry(detached).State.ToString();

        return new ChangeTrackingDemoDto(initial, afterAttach, afterSave);
    }
}
