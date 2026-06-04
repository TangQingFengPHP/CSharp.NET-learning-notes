using EfCorePractice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Application.Abstractions;

/// <summary>
/// 读库上下文：只读查询，配合 NoTracking 使用。
/// </summary>
public interface IAppReadDbContext
{
    DbSet<User> Users { get; }

    DbSet<Order> Orders { get; }
}
