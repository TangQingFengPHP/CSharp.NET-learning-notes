using EfCorePractice.Application.Abstractions;
using EfCorePractice.Application.Models;
using EfCorePractice.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Application.Services;

public class OrderService(IApplicationDbContext db)
{
    public Task<List<OrderUserDto>> FindOrderUserByStatusAsync(OrderStatus status, CancellationToken ct = default) =>
        db.Orders.AsNoTracking()
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.Id)
            .Select(o => new OrderUserDto(
                o.Id,
                o.OrderNo,
                o.Amount,
                o.Status,
                o.User.Id,
                o.User.Username,
                o.User.Email))
            .ToListAsync(ct);

    public Task<Dictionary<OrderStatus, int>> CountByStatusAsync(CancellationToken ct = default) =>
        db.Orders.AsNoTracking()
            .GroupBy(o => o.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);
}
