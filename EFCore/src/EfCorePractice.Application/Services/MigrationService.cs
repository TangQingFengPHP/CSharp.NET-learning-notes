using EfCorePractice.Application.Abstractions;
using EfCorePractice.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Application.Services;

public class MigrationService(IApplicationDbContext db)
{
    public async Task<MigrationInfoDto> GetMigrationInfoAsync(CancellationToken ct = default)
    {
        var provider = db.Database.ProviderName ?? "unknown";

        if (provider.Contains("InMemory", StringComparison.OrdinalIgnoreCase))
        {
            return new MigrationInfoDto(
                provider,
                ["(InMemory 不支持迁移历史)"],
                []);
        }

        var applied = await db.Database.GetAppliedMigrationsAsync(ct);
        var pending = await db.Database.GetPendingMigrationsAsync(ct);

        return new MigrationInfoDto(provider, applied.ToList(), pending.ToList());
    }
}
