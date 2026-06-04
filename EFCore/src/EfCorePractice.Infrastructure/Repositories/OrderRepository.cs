using EfCorePractice.Application.Abstractions.Repositories;
using EfCorePractice.Domain.Entities;
using EfCorePractice.Infrastructure.Persistence;

namespace EfCorePractice.Infrastructure.Repositories;

public class OrderRepository(AppDbContext db) : IOrderRepository
{
    public Task AddAsync(Order order, CancellationToken ct = default)
    {
        db.Orders.Add(order);
        return Task.CompletedTask;
    }
}
