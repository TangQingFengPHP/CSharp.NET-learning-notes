using EfCorePractice.Application.Abstractions;
using EfCorePractice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Infrastructure.Persistence;

/// <summary>
/// 读库 DbContext（模式 C：读写分离，默认 NoTracking，无 SaveChanges 业务扩展）。
/// </summary>
public class AppReadDbContext(
    DbContextOptions<AppReadDbContext> options,
    ITenantContext tenantContext) : DbContext(options), IAppReadDbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        AppDbContextModelConfiguration.Configure(modelBuilder, tenantContext);
}
