using EfCorePractice.Application.Abstractions;
using EfCorePractice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Infrastructure.Persistence;

internal static class CompiledQueries
{
    public static readonly Func<AppDbContext, long, Task<User?>> GetUserById =
        EF.CompileAsyncQuery((AppDbContext context, long id) =>
            context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id));
}

public sealed class ScopedCompiledQueryService(AppDbContext context) : ICompiledQueryService
{
    public Task<User?> GetUserByIdAsync(long id, CancellationToken cancellationToken = default) =>
        CompiledQueries.GetUserById(context, id);
}
