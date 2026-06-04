using EfCorePractice.Application.Abstractions;
using EfCorePractice.Application.Models;
using EfCorePractice.Application.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EfCorePractice.Application.Services;

public class PatternsService(
    IOptions<SlowQueryOptions> slowQueryOptions,
    ISlowQueryMetrics metrics,
    IApplicationDbContext db)
{
    public PatternsOverviewDto GetOverview() =>
        new(
            [
                new PatternModeDto(
                    "A",
                    "Service + DbContext",
                    "应用服务直接注入 IApplicationDbContext，最常见。",
                    "/users",
                    "AppDbContext（写）"),
                new PatternModeDto(
                    "B",
                    "Repository + UnitOfWork",
                    "仓储封装 DbSet，工作单元统一 Commit。",
                    "/patterns/uow/users",
                    "AppDbContext（写，经 UnitOfWork）"),
                new PatternModeDto(
                    "C",
                    "Read/Write DbContext Split",
                    "读库 AppReadDbContext 默认 NoTracking，写库仍用 AppDbContext。",
                    "/patterns/read/users",
                    "AppReadDbContext（读） / AppDbContext（写）")
            ],
            [
                "1. SlowQueryInterceptor（DbCommandInterceptor，记录慢 SQL）",
                "2. AuditableEntityInterceptor（SaveChangesInterceptor，审计字段）"
            ]);

    public SlowQueryStatsDto GetSlowQueryStats() => metrics.GetStats();

    public async Task<SlowQueryLogDto> TriggerSlowQueryDemoAsync(CancellationToken ct = default)
    {
        var threshold = slowQueryOptions.Value.ThresholdMs;

        if (db.Database.IsRelational())
        {
            var seconds = Math.Max(threshold / 1000.0, 0.15);
            await db.Database.ExecuteSqlAsync($"SELECT SLEEP({seconds})", ct);
        }
        else
        {
            await Task.Delay(threshold + 50, ct);
        }

        _ = await db.Users.CountAsync(ct);

        var stats = metrics.GetStats();
        return stats.Recent.FirstOrDefault()
               ?? new SlowQueryLogDto(
                   DateTime.UtcNow,
                   "Demo",
                   threshold + 50,
                   threshold,
                   "InMemory demo delay");
    }
}
