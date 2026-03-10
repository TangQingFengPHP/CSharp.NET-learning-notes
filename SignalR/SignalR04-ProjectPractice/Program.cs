using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<OrderTicker>();
builder.Services.AddHostedService<OrderStatusBroadcastService>();

var app = builder.Build();

app.MapGet("/", () => Results.Text(
    "SignalR 04 ProjectPractice\n" +
    "Hub route: /hubs/order-notification\n" +
    "HTTP route: GET /orders/latest\n" +
    "说明: 后台服务会定时广播订单状态变更。"));

app.MapGet("/orders/latest", (OrderTicker ticker) => Results.Ok(ticker.Snapshot()));

app.MapHub<OrderNotificationHub>("/hubs/order-notification");

app.Run();

public sealed class OrderNotificationHub : Hub
{
}

public sealed class OrderTicker
{
    private readonly string[] _statuses = ["Pending", "Paid", "Shipped", "Completed"];
    private int _index;

    public OrderStatusSnapshot Next()
    {
        var status = _statuses[_index % _statuses.Length];
        _index++;
        return new OrderStatusSnapshot(1001, status, DateTimeOffset.Now);
    }

    public object Snapshot()
    {
        return new { orderId = 1001, nextStatus = _statuses[_index % _statuses.Length] };
    }
}

public sealed class OrderStatusBroadcastService(
    IHubContext<OrderNotificationHub> hubContext,
    OrderTicker ticker,
    ILogger<OrderStatusBroadcastService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var snapshot = ticker.Next();

            logger.LogInformation("Broadcasting order update: {@Snapshot}", snapshot);
            await hubContext.Clients.All.SendAsync("OrderStatusUpdated", snapshot, stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}

public sealed record OrderStatusSnapshot(long OrderId, string Status, DateTimeOffset UpdatedAt);
