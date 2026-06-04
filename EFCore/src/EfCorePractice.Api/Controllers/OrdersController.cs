using EfCorePractice.Application.Models;
using EfCorePractice.Application.Services;
using EfCorePractice.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EfCorePractice.Api.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(OrderService orderService) : ControllerBase
{
    [HttpGet("join")]
    public Task<List<OrderUserDto>> Join([FromQuery] OrderStatus status = OrderStatus.Paid, CancellationToken ct = default) =>
        orderService.FindOrderUserByStatusAsync(status, ct);

    [HttpGet("stats")]
    public Task<Dictionary<OrderStatus, int>> Stats(CancellationToken ct) =>
        orderService.CountByStatusAsync(ct);
}
