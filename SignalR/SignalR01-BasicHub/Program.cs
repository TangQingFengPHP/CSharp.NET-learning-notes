using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapGet("/", () => Results.Text(
    "SignalR 01 BasicHub\n" +
    "Hub route: /hubs/chat\n" +
    "HTTP debug route: POST /debug/broadcast?user=alice&message=hello\n" +
    "说明: 真实客户端可连接 Hub 后监听 ReceiveMessage 事件。"));

app.MapPost("/debug/broadcast", async (
    string user,
    string message,
    IHubContext<ChatHub> hubContext) =>
{
    await hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
    return Results.Ok(new { sent = true, user, message });
});

app.MapHub<ChatHub>("/hubs/chat");

app.Run();

public sealed class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveSystemMessage", $"connected:{Context.ConnectionId}");
        await base.OnConnectedAsync();
    }
}
