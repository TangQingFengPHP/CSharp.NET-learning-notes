using EfCorePractice.Application.Abstractions;
using EfCorePractice.Domain.Common;
using EfCorePractice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Infrastructure.Persistence;

/// <summary>
/// 写库 DbContext（模式 A：Service 直用 DbContext 的默认实现，亦供仓储/工作单元写入）。
/// </summary>
public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ITenantContext tenantContext) : DbContext(options), IApplicationDbContext, IAppWriteDbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        AppDbContextModelConfiguration.Configure(modelBuilder, tenantContext);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplySoftDelete();
        ApplyTenantOnInsert();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplySoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = DateTime.UtcNow;
        }
    }

    private void ApplyTenantOnInsert()
    {
        if (!tenantContext.HasTenant)
        {
            return;
        }

        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.TenantId = tenantContext.TenantId;
            }
        }
    }
}
