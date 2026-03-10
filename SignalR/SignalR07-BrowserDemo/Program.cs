using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new
{
    ok = true,
    serverTime = DateTimeOffset.Now,
    hub = "/hubs/live-chat"
}));

app.MapPost("/api/system-message", async (string message, IHubContext<LiveChatHub> hubContext) =>
{
    await hubContext.Clients.All.SendAsync("ReceiveMessage", "system", message);
    return Results.Ok(new { sent = true, sender = "system", message });
});

app.MapHub<LiveChatHub>("/hubs/live-chat");

app.Run();

public sealed class LiveChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveSystemMessage", $"connected:{Context.ConnectionId}");
        await Clients.All.SendAsync("ReceiveSystemMessage", $"online:{Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.SendAsync("ReceiveSystemMessage", $"offline:{Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
