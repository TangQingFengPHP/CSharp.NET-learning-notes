namespace EfCorePractice.Application.Abstractions.Repositories;

/// <summary>
/// 工作单元：聚合多个仓储，统一 Commit。
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository Users { get; }

    IOrderRepository Orders { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
