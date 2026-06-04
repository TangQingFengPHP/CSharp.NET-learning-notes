using EfCorePractice.Domain.Entities;

namespace EfCorePractice.Application.Abstractions.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct = default);
}
