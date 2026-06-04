using EfCorePractice.Application.Abstractions.Repositories;
using EfCorePractice.Infrastructure.Persistence;

namespace EfCorePractice.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    private IUserRepository? _users;
    private IOrderRepository? _orders;

    public IUserRepository Users => _users ??= new UserRepository(db);

    public IOrderRepository Orders => _orders ??= new OrderRepository(db);

    public Task<int> CommitAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
