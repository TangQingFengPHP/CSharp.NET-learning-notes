using EfCorePractice.Application.Abstractions.Repositories;
using EfCorePractice.Domain.Entities;
using EfCorePractice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task AddAsync(User user, CancellationToken ct = default)
    {
        db.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task<User?> GetByIdAsync(long id, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public void Update(User user) => db.Users.Update(user);

    public Task<bool> ExistsAsync(long id, CancellationToken ct = default) =>
        db.Users.AnyAsync(u => u.Id == id, ct);
}
