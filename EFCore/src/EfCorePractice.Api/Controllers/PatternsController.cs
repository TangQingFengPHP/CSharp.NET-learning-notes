using EfCorePractice.Application.Models;
using EfCorePractice.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EfCorePractice.Api.Controllers;

/// <summary>
/// 三种数据访问模式对照 + 拦截器链观测。
/// </summary>
[ApiController]
[Route("patterns")]
public class PatternsController(PatternsService patternsService) : ControllerBase
{
    [HttpGet]
    public PatternsOverviewDto Overview() => patternsService.GetOverview();

    [HttpGet("interceptors/slow-queries")]
    public SlowQueryStatsDto SlowQueries() => patternsService.GetSlowQueryStats();

    [HttpPost("interceptors/slow-queries/demo")]
    public Task<SlowQueryLogDto> TriggerSlowQueryDemo(CancellationToken ct) =>
        patternsService.TriggerSlowQueryDemoAsync(ct);
}
