using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
    })
    .AddHubOptions<PipelineDemoHub>(options =>
    {
        options.AddFilter<LoggingHubFilter>();
    });

builder.Services.AddSingleton<LoggingHubFilter>();

var app = builder.Build();

app.MapGet("/", () => Results.Text(
    "SignalR 05 PrincipleAndPipeline\n" +
    "Hub route: /hubs/pipeline\n" +
    "说明: 观察 OnConnectedAsync、IHubFilter、Hub 方法调用之间的职责边界。"));

app.MapHub<PipelineDemoHub>("/hubs/pipeline");

app.Run();

public sealed class PipelineDemoHub(ILogger<PipelineDemoHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("Connection established: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public async Task<string> Echo(string input)
    {
        logger.LogInformation("Hub method executing: Echo, input={Input}", input);
        await Clients.Caller.SendAsync("EchoReceived", input);
        return $"echo:{input}";
    }
}

public sealed class LoggingHubFilter(ILogger<LoggingHubFilter> logger) : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        logger.LogInformation(
            "Before hub method: {Hub}.{Method}",
            invocationContext.Hub.GetType().Name,
            invocationContext.HubMethodName);

        var result = await next(invocationContext);

        logger.LogInformation(
            "After hub method: {Hub}.{Method}",
            invocationContext.Hub.GetType().Name,
            invocationContext.HubMethodName);

        return result;
    }

    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        logger.LogInformation("Filter OnConnected: {ConnectionId}", context.Context.ConnectionId);
        await next(context);
    }

    public async Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
    {
        logger.LogInformation("Filter OnDisconnected: {ConnectionId}", context.Context.ConnectionId);
        await next(context, exception);
    }
}
