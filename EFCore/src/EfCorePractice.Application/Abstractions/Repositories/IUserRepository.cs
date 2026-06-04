using EfCorePractice.Domain.Entities;

namespace EfCorePractice.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct = default);

    Task<User?> GetByIdAsync(long id, CancellationToken ct = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    void Update(User user);

    Task<bool> ExistsAsync(long id, CancellationToken ct = default);
}
