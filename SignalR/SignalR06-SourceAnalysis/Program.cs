using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<DemoHubLifetimeManager>();
builder.Services.AddSingleton<DemoHubDispatcher>();

var app = builder.Build();

app.MapGet("/", (DemoHubDispatcher dispatcher) =>
{
    var summary = dispatcher.Describe();
    return Results.Text(summary);
});

app.MapPost("/debug/dispatch", async (
    DemoHubDispatcher dispatcher,
    DemoHubLifetimeManager lifetimeManager) =>
{
    var method = "SendDashboardUpdate";
    var payload = new { TotalUsers = 128, ActiveUsers = 35 };

    dispatcher.RecordInvocation(method);
    await lifetimeManager.BroadcastAsync(method, payload);

    return Results.Ok(new
    {
        dispatcher = dispatcher.DescribeAsObject(),
        lifetimeManager = lifetimeManager.Snapshot()
    });
});

app.MapHub<DashboardHub>("/hubs/dashboard");

app.Run();

public sealed class DashboardHub : Hub
{
    public async Task SendDashboardUpdate(object payload)
    {
        await Clients.All.SendAsync("DashboardUpdated", payload);
    }
}

public sealed class DemoHubDispatcher
{
    private readonly List<string> _invocations = [];

    public void RecordInvocation(string methodName)
    {
        _invocations.Add(methodName);
    }

    public string Describe()
    {
        return "SignalR 06 SourceAnalysis\n" +
               "Hub route: /hubs/dashboard\n" +
               "POST /debug/dispatch 用于模拟 Dispatcher -> LifetimeManager 的职责分工\n" +
               $"Recorded invocations: {string.Join(", ", _invocations.DefaultIfEmpty("<none>"))}";
    }

    public object DescribeAsObject()
    {
        return new
        {
            role = "负责将一次调用路由到目标 Hub 方法",
            invocations = _invocations.ToArray()
        };
    }
}

public sealed class DemoHubLifetimeManager
{
    private readonly List<string> _messages = [];

    public Task BroadcastAsync(string methodName, object payload)
    {
        _messages.Add($"broadcast::{methodName}::{payload}");
        return Task.CompletedTask;
    }

    public object Snapshot()
    {
        return new
        {
            role = "负责连接与消息分发的生命周期管理",
            messages = _messages.ToArray()
        };
    }
}
