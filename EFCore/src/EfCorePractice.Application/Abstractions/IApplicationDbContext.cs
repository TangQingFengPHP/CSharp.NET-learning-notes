using EfCorePractice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EfCorePractice.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    DbSet<Order> Orders { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
