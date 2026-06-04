using EfCorePractice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Application.Abstractions;

/// <summary>
/// 写库上下文标记接口（与 <see cref="IApplicationDbContext"/> 共用 AppDbContext 实现）。
/// </summary>
public interface IAppWriteDbContext
{
    DbSet<User> Users { get; }

    DbSet<Order> Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
