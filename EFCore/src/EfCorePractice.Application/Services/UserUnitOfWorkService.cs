using EfCorePractice.Application.Abstractions;
using EfCorePractice.Application.Abstractions.Repositories;
using EfCorePractice.Application.Models;
using EfCorePractice.Domain.Entities;
using EfCorePractice.Domain.Enums;

namespace EfCorePractice.Application.Services;

/// <summary>
/// 模式 B：仓储 + 工作单元（与 <see cref="UserService"/> 直用 DbContext 对照）。
/// </summary>
public class UserUnitOfWorkService(IUnitOfWork unitOfWork, ITenantContext tenantContext)
{
    public async Task<long> CreateAsync(
        string username,
        string email,
        int age,
        UserContactProfile? contact = null,
        CancellationToken ct = default)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            Age = age,
            Status = "ACTIVE",
            TenantId = tenantContext.HasTenant ? tenantContext.TenantId : 1,
            Contact = contact ?? new UserContactProfile { Email = email }
        };

        await unitOfWork.Users.AddAsync(user, ct);
        await unitOfWork.CommitAsync(ct);
        return user.Id;
    }

    public Task<User?> GetByIdAsync(long id, CancellationToken ct = default) =>
        unitOfWork.Users.GetByIdAsync(id, ct);

    public async Task UpdateEmailAsync(long id, string email, CancellationToken ct = default)
    {
        var user = await unitOfWork.Users.GetByIdAsync(id, ct)
                   ?? throw new InvalidOperationException("用户不存在");

        user.Email = email;
        user.Contact.Email = email;
        unitOfWork.Users.Update(user);
        await unitOfWork.CommitAsync(ct);
    }

    public async Task<TransactionDemoResult> CreateUserWithOrderAsync(
        CreateUserWithOrderRequest request,
        CancellationToken ct = default)
    {
        var user = new User
        {
            Username = request.User.Username,
            Email = request.User.Email,
            Age = request.User.Age,
            Status = "ACTIVE",
            TenantId = tenantContext.HasTenant ? tenantContext.TenantId : 1,
            Contact = request.User.Contact ?? new UserContactProfile { Email = request.User.Email }
        };

        await unitOfWork.Users.AddAsync(user, ct);
        await unitOfWork.CommitAsync(ct);

        var order = new Order
        {
            UserId = user.Id,
            OrderNo = request.OrderNo,
            Amount = request.Amount,
            Status = OrderStatus.Paid,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Orders.AddAsync(order, ct);
        await unitOfWork.CommitAsync(ct);

        return new TransactionDemoResult(
            user.Id,
            order.Id,
            "工作单元：用户与订单分两次 Commit（同一 Scoped DbContext）");
    }
}
