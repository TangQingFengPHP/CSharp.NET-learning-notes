using System.Collections.Concurrent;
using EfCorePractice.Application.Abstractions;
using EfCorePractice.Application.Models;
using EfCorePractice.Application.Options;
using Microsoft.Extensions.Options;

namespace EfCorePractice.Infrastructure.Interceptors;

public sealed class SlowQueryMetrics(IOptions<SlowQueryOptions> options) : ISlowQueryMetrics
{
    private readonly ConcurrentQueue<SlowQueryLogDto> _recent = new();

    public int ThresholdMs => options.Value.ThresholdMs;

    public void Record(SlowQueryLogDto entry)
    {
        _recent.Enqueue(entry);

        var capacity = options.Value.RecentLogCapacity;
        while (_recent.Count > capacity && _recent.TryDequeue(out _))
        {
        }
    }

    public SlowQueryStatsDto GetStats() =>
        new(
            ThresholdMs,
            _recent.Count,
            _recent.Reverse().ToList());
}
