using EfCorePractice.Domain.Entities;

namespace EfCorePractice.Application.Abstractions;

public interface ICompiledQueryService
{
    Task<User?> GetUserByIdAsync(long id, CancellationToken cancellationToken = default);
}
