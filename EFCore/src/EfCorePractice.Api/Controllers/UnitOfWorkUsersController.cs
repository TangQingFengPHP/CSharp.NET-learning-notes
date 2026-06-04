using EfCorePractice.Application.Models;
using EfCorePractice.Application.Services;
using EfCorePractice.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EfCorePractice.Api.Controllers;

/// <summary>
/// 模式 B：仓储 + 工作单元（对照 <see cref="UsersController"/> 模式 A）。
/// </summary>
[ApiController]
[Route("patterns/uow/users")]
public class UnitOfWorkUsersController(UserUnitOfWorkService service) : ControllerBase
{
    [HttpPost]
    public Task<long> Create([FromBody] UserCreateRequest req, CancellationToken ct) =>
        service.CreateAsync(req.Username, req.Email, req.Age, req.Contact, ct);

    [HttpGet("{id:long}")]
    public async Task<User> Detail(long id, CancellationToken ct) =>
        await service.GetByIdAsync(id, ct) ?? throw new InvalidOperationException("用户不存在");

    [HttpPut("{id:long}/email")]
    public Task UpdateEmail(long id, [FromBody] UserUpdateEmailRequest req, CancellationToken ct) =>
        service.UpdateEmailAsync(id, req.Email, ct);

    [HttpPost("with-order")]
    public Task<TransactionDemoResult> CreateWithOrder([FromBody] CreateUserWithOrderRequest req, CancellationToken ct) =>
        service.CreateUserWithOrderAsync(req, ct);
}
