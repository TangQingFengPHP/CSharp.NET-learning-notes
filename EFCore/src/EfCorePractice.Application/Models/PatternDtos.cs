namespace EfCorePractice.Application.Models;

public record PatternModeDto(
    string Code,
    string Name,
    string Description,
    string RoutePrefix,
    string DbContext);

public record PatternsOverviewDto(
    IReadOnlyList<PatternModeDto> Modes,
    IReadOnlyList<string> InterceptorChain);

public record SlowQueryLogDto(
    DateTime OccurredAt,
    string ContextType,
    long DurationMs,
    int ThresholdMs,
    string CommandText);

public record SlowQueryStatsDto(
    int ThresholdMs,
    int TotalLogged,
    IReadOnlyList<SlowQueryLogDto> Recent);
