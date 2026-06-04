using EfCorePractice.Application.Models;

namespace EfCorePractice.Application.Abstractions;

public interface ISlowQueryMetrics
{
    int ThresholdMs { get; }

    void Record(SlowQueryLogDto entry);

    SlowQueryStatsDto GetStats();
}
