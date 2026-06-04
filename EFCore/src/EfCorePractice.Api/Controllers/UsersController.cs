using EfCorePractice.Application.Models;
using EfCorePractice.Application.Services;
using EfCorePractice.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EfCorePractice.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController(UserService userService) : ControllerBase
{
    [HttpPost]
    public Task<long> Create([FromBody] UserCreateRequest req, CancellationToken ct) =>
        userService.CreateAsync(req.Username, req.Email, req.Age, req.Contact, ct);

    [HttpGet("{id:long}")]
    public async Task<User> Detail(long id, CancellationToken ct) =>
        await userService.FindByIdAsync(id, ct) ?? throw new InvalidOperationException("用户不存在");

    [HttpGet("{id:long}/with-orders")]
    public async Task<UserWithOrdersDto> DetailWithOrders(long id, CancellationToken ct) =>
        await userService.FindWithOrdersByIdAsync(id, splitQuery: false, ct)
        ?? throw new InvalidOperationException("用户不存在");

    [HttpGet("{id:long}/with-orders-split")]
    public async Task<UserWithOrdersDto> DetailWithOrdersSplit(long id, CancellationToken ct) =>
        await userService.FindWithOrdersByIdAsync(id, splitQuery: true, ct)
        ?? throw new InvalidOperationException("用户不存在");

    [HttpGet("by-email")]
    public async Task<User> FindByEmail([FromQuery] string email, CancellationToken ct) =>
        await userService.FindByEmailAsync(email, ct) ?? throw new InvalidOperationException("用户不存在");

    [HttpGet("linq")]
    public Task<List<User>> SearchByLinQ([FromQuery] string status = "ACTIVE", CancellationToken ct = default) =>
        userService.SearchByLinQAsync(status, ct);

    [HttpPost("search")]
    public Task<PageResult<User>> SearchPage(
        [FromBody] UserSearchRequest? req,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default) =>
        userService.PageAsync(req?.Keyword, req?.Status, req?.MinAge, pageNumber, pageSize, ct);

    [HttpGet]
    public Task<PageResult<User>> Page(
        [FromQuery] string status = "ACTIVE",
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default) =>
        userService.PageAsync(null, status, null, pageNumber, pageSize, ct);

    [HttpGet("slice")]
    public Task<SliceResult<User>> Slice(
        [FromQuery] string status = "ACTIVE",
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default) =>
        userService.SliceByStatusAsync(status, pageNumber, pageSize, ct);

    [HttpGet("jpql")]
    public Task<List<User>> FindByLinQ(
        [FromQuery] string status = "ACTIVE",
        [FromQuery] int minAge = 18,
        CancellationToken ct = default) =>
        userService.FindByLinQAsync(status, minAge, ct);

    [HttpGet("native")]
    public Task<List<User>> FindByNative(
        [FromQuery] string status = "ACTIVE",
        [FromQuery] int minAge = 18,
        CancellationToken ct = default) =>
        userService.FindByNativeSqlAsync(status, minAge, ct);

    [HttpGet("summary")]
    public Task<List<UserSummaryDto>> Summaries([FromQuery] string status = "ACTIVE", CancellationToken ct = default) =>
        userService.FindSummariesAsync(status, ct);

    [HttpGet("by-ids")]
    public Task<List<User>> FindByIds([FromQuery] string ids, CancellationToken ct)
    {
        var idList = ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToList();
        return userService.FindByIdsAsync(idList, ct);
    }

    [HttpPut("{id:long}/email")]
    public Task UpdateEmail(long id, [FromBody] UserUpdateEmailRequest req, CancellationToken ct) =>
        userService.UpdateEmailAsync(id, req.Email, ct);

    [HttpPut("{id:long}/disable")]
    public Task Disable(long id, CancellationToken ct) =>
        userService.DisableAsync(id, ct);

    [HttpPut("disable-by-age")]
    public Task DisableByAge([FromQuery] int ltAge = 18, CancellationToken ct = default) =>
        userService.DisableByAgeLessThanAsync(ltAge, ct);

    [HttpPut("{id:long}/optimistic-disable")]
    public Task OptimisticDisable(long id, CancellationToken ct) =>
        userService.OptimisticDisableAsync(id, ct);

    [HttpDelete("{id:long}/soft")]
    public Task SoftDelete(long id, CancellationToken ct) =>
        userService.SoftDeleteAsync(id, ct);

    [HttpGet("deleted")]
    public Task<List<User>> ListDeleted(CancellationToken ct) =>
        userService.FindDeletedAsync(ct);

    [HttpPut("{id:long}/restore")]
    public Task Restore(long id, CancellationToken ct) =>
        userService.RestoreSoftDeletedAsync(id, ct);

    [HttpDelete("{id:long}")]
    public Task HardDelete(long id, CancellationToken ct) =>
        userService.HardDeleteAsync(id, ct);
}
