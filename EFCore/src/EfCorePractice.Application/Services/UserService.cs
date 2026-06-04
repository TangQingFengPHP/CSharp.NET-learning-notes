using EfCorePractice.Application.Abstractions;
using EfCorePractice.Application.Models;
using EfCorePractice.Application.Specifications;
using EfCorePractice.Domain.Entities;
using EfCorePractice.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Application.Services;

public class UserService(IApplicationDbContext db, ITenantContext tenantContext)
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

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user.Id;
    }

    public Task<User?> FindByIdAsync(long id, CancellationToken ct = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<UserWithOrdersDto?> FindWithOrdersByIdAsync(long id, bool splitQuery, CancellationToken ct = default)
    {
        IQueryable<User> query = db.Users.AsNoTracking().Include(u => u.Orders);
        if (splitQuery)
        {
            query = query.AsSplitQuery();
        }

        var user = await query.FirstOrDefaultAsync(u => u.Id == id, ct);
        return user is null ? null : MapWithOrders(user);
    }

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<List<User>> SearchByLinQAsync(string status, CancellationToken ct = default) =>
        db.Users.AsNoTracking()
            .Where(u => u.Status == status)
            .OrderByDescending(u => u.Id)
            .ToListAsync(ct);

    public Task<List<User>> FindByIdsAsync(IReadOnlyList<long> ids, CancellationToken ct = default) =>
        db.Users.AsNoTracking()
            .Where(u => ids.Contains(u.Id))
            .OrderBy(u => u.Id)
            .ToListAsync(ct);

    public Task<List<User>> FindByLinQAsync(string status, int minAge, CancellationToken ct = default) =>
        db.Users.AsNoTracking()
            .Where(u => u.Status == status && u.Age >= minAge)
            .OrderByDescending(u => u.Id)
            .ToListAsync(ct);

    public Task<List<User>> FindByNativeSqlAsync(string status, int minAge, CancellationToken ct = default) =>
        db.Users.FromSqlInterpolated($"""
            SELECT id, username, email, age, status, version, tenant_id, is_deleted, deleted_at,
                   created_at, updated_at, contact
            FROM users
            WHERE status = {status} AND age >= {minAge} AND is_deleted = 0
            ORDER BY id DESC
            """)
            .AsNoTracking()
            .ToListAsync(ct);

    public Task<List<UserSummaryDto>> FindSummariesAsync(string status, CancellationToken ct = default) =>
        db.Users.AsNoTracking()
            .Where(u => u.Status == status)
            .OrderByDescending(u => u.Id)
            .Select(u => new UserSummaryDto(u.Id, u.Username, u.Email))
            .ToListAsync(ct);

    public async Task<PageResult<User>> PageAsync(
        string? keyword,
        string? status,
        int? minAge,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = db.Users.AsNoTracking().ApplySearch(keyword, status, minAge);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(u => u.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return PageResult<User>.From(items, total, pageNumber, pageSize);
    }

    public async Task<SliceResult<User>> SliceByStatusAsync(
        string status,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = db.Users.AsNoTracking().Where(u => u.Status == status);
        var buffer = await query
            .OrderByDescending(u => u.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize + 1)
            .ToListAsync(ct);

        var hasNext = buffer.Count > pageSize;
        var items = hasNext ? buffer.Take(pageSize).ToList() : buffer;

        return SliceResult<User>.From(items, pageNumber, pageSize, hasNext);
    }

    public async Task UpdateEmailAsync(long id, string email, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
                   ?? throw new InvalidOperationException("用户不存在");

        user.Email = email;
        user.Contact.Email = email;
        await db.SaveChangesAsync(ct);
    }

    public async Task DisableAsync(long id, CancellationToken ct = default)
    {
        var updated = await db.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(u => u.Status, "DISABLED")
                    .SetProperty(u => u.UpdatedAt, DateTime.UtcNow),
                ct);

        if (updated == 0)
        {
            throw new InvalidOperationException("用户不存在");
        }
    }

    public Task DisableByAgeLessThanAsync(int ageExclusive, CancellationToken ct = default) =>
        db.Users
            .Where(u => u.Age < ageExclusive)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(u => u.Status, "DISABLED")
                    .SetProperty(u => u.UpdatedAt, DateTime.UtcNow),
                ct);

    public async Task OptimisticDisableAsync(long id, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
                   ?? throw new InvalidOperationException("用户不存在");

        user.Status = "DISABLED";
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(long id, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
                   ?? throw new InvalidOperationException("用户不存在");

        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }

    public Task<List<User>> FindDeletedAsync(CancellationToken ct = default) =>
        db.Users.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(u => u.IsDeleted)
            .OrderByDescending(u => u.DeletedAt)
            .ToListAsync(ct);

    public async Task RestoreSoftDeletedAsync(long id, CancellationToken ct = default)
    {
        var updated = await db.Users.IgnoreQueryFilters()
            .Where(u => u.Id == id && u.IsDeleted)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(u => u.IsDeleted, false)
                    .SetProperty(u => u.DeletedAt, (DateTime?)null)
                    .SetProperty(u => u.UpdatedAt, DateTime.UtcNow),
                ct);

        if (updated == 0)
        {
            throw new InvalidOperationException("软删除用户不存在");
        }
    }

    public async Task HardDeleteAsync(long id, CancellationToken ct = default)
    {
        var deleted = await db.Users.IgnoreQueryFilters()
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(ct);

        if (deleted == 0)
        {
            throw new InvalidOperationException("用户不存在");
        }
    }

    public Task<int> CountActiveAsync(CancellationToken ct = default) =>
        db.Users.AsNoTracking().CountAsync(ct);

    private static UserWithOrdersDto MapWithOrders(User user) =>
        new(
            user.Id,
            user.Username,
            user.Email,
            user.Age,
            user.Status,
            user.TenantId,
            new UserContactDto(user.Contact.Email, user.Contact.Phone, user.Contact.Tags),
            user.Orders
                .OrderByDescending(o => o.Id)
                .Select(o => new UserOrderItemDto(
                    o.Id, o.OrderNo, o.Amount, o.Status, o.CreatedAt))
                .ToList());
}
