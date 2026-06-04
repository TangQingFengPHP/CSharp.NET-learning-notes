using EfCorePractice.Application.Models;
using EfCorePractice.Application.Services;
using EfCorePractice.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EfCorePractice.Api.Controllers;

[ApiController]
[Route("demo")]
public class DemoController(DemoService demoService, UserService userService) : ControllerBase
{
    [HttpGet("query-plan")]
    public QueryPlanDto QueryPlan([FromQuery] int minAge = 18) =>
        demoService.GetUserQueryPlan(minAge);

    [HttpGet("compiled/{id:long}")]
    public async Task<User> Compiled(long id, CancellationToken ct) =>
        await demoService.GetUserCompiledAsync(id, ct) ?? throw new InvalidOperationException("用户不存在");

    [HttpPost("transaction")]
    public Task<TransactionDemoResult> Transaction([FromBody] CreateUserWithOrderRequest req, CancellationToken ct) =>
        demoService.CreateUserWithOrderInTransactionAsync(req, ct);

    [HttpPost("change-tracking")]
    public Task<ChangeTrackingDemoDto> ChangeTracking(CancellationToken ct) =>
        demoService.DemonstrateChangeTrackingAsync(ct);

    [HttpGet("tenant-count")]
    public async Task<object> TenantCount(CancellationToken ct) =>
        new { ActiveUsers = await userService.CountActiveAsync(ct) };
}
