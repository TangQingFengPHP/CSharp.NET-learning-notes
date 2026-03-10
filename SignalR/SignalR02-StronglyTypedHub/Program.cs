using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapGet("/", () => Results.Text(
    "SignalR 02 StronglyTypedHub\n" +
    "Hub route: /hubs/clock\n" +
    "HTTP debug route: POST /debug/time\n" +
    "说明: 通过 Hub<IClockClient> 消除字符串事件名。"));

app.MapPost("/debug/time", async (IHubContext<ClockHub, IClockClient> hubContext) =>
{
    var payload = $"server-time:{DateTimeOffset.Now:O}";
    await hubContext.Clients.All.ReceiveCurrentTime(payload);
    return Results.Ok(new { sent = true, payload });
});

app.MapHub<ClockHub>("/hubs/clock");

app.Run();

public interface IClockClient
{
    Task ReceiveCurrentTime(string currentTime);

    Task ReceiveAnnouncement(string message);
}

public sealed class ClockHub : Hub<IClockClient>
{
    public async Task BroadcastServerTime()
    {
        await Clients.All.ReceiveCurrentTime(DateTimeOffset.UtcNow.ToString("O"));
    }

    public async Task Announce(string message)
    {
        await Clients.All.ReceiveAnnouncement(message);
    }
}
