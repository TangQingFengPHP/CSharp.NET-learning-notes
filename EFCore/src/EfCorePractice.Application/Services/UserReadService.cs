using EfCorePractice.Application.Abstractions;
using EfCorePractice.Application.Models;
using EfCorePractice.Application.Specifications;
using EfCorePractice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Application.Services;

/// <summary>
/// 模式 C：读写分离之读侧（<see cref="IAppReadDbContext"/> + 默认 NoTracking）。
/// 写操作请仍走 <see cref="UserService"/> 或模式 B。
/// </summary>
public class UserReadService(IAppReadDbContext readDb)
{
    public Task<User?> FindByIdAsync(long id, CancellationToken ct = default) =>
        readDb.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<UserWithOrdersDto?> FindWithOrdersByIdAsync(long id, CancellationToken ct = default)
    {
        var user = await readDb.Users
            .Include(u => u.Orders)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null)
        {
            return null;
        }

        return new UserWithOrdersDto(
            user.Id,
            user.Username,
            user.Email,
            user.Age,
            user.Status,
            user.TenantId,
            new UserContactDto(user.Contact.Email, user.Contact.Phone, user.Contact.Tags),
            user.Orders
                .OrderByDescending(o => o.Id)
                .Select(o => new UserOrderItemDto(o.Id, o.OrderNo, o.Amount, o.Status, o.CreatedAt))
                .ToList());
    }

    public Task<List<UserSummaryDto>> FindSummariesAsync(string status, CancellationToken ct = default) =>
        readDb.Users
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
        var query = readDb.Users.ApplySearch(keyword, status, minAge);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(u => u.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return PageResult<User>.From(items, total, pageNumber, pageSize);
    }
}
