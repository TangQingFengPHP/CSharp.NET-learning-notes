using EfCorePractice.Application.Models;
using EfCorePractice.Application.Services;
using EfCorePractice.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EfCorePractice.Api.Controllers;

/// <summary>
/// 模式 C：读写分离 - 读库（对照 <see cref="UsersController"/> 写/通用查询）。
/// </summary>
[ApiController]
[Route("patterns/read/users")]
public class ReadUsersController(UserReadService readService) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<User> Detail(long id, CancellationToken ct) =>
        await readService.FindByIdAsync(id, ct) ?? throw new InvalidOperationException("用户不存在");

    [HttpGet("{id:long}/with-orders")]
    public async Task<UserWithOrdersDto> DetailWithOrders(long id, CancellationToken ct) =>
        await readService.FindWithOrdersByIdAsync(id, ct)
        ?? throw new InvalidOperationException("用户不存在");

    [HttpGet("summary")]
    public Task<List<UserSummaryDto>> Summaries([FromQuery] string status = "ACTIVE", CancellationToken ct = default) =>
        readService.FindSummariesAsync(status, ct);

    [HttpGet]
    public Task<PageResult<User>> Page(
        [FromQuery] string? status = "ACTIVE",
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default) =>
        readService.PageAsync(null, status, null, pageNumber, pageSize, ct);

    [HttpPost("search")]
    public Task<PageResult<User>> SearchPage(
        [FromBody] UserSearchRequest? req,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default) =>
        readService.PageAsync(req?.Keyword, req?.Status, req?.MinAge, pageNumber, pageSize, ct);
}
