using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapGet("/", () => Results.Text(
    "SignalR 03 GroupsAndUsers\n" +
    "Hub route: /hubs/project-room\n" +
    "HTTP debug route: POST /debug/group/{groupName}?message=hello\n" +
    "说明: 演示组广播和按用户推送的接口形态。"));

app.MapPost("/debug/group/{groupName}", async (
    string groupName,
    string message,
    IHubContext<ProjectRoomHub> hubContext) =>
{
    await hubContext.Clients.Group(groupName).SendAsync("ReceiveGroupMessage", groupName, message);
    return Results.Ok(new { sent = true, groupName, message });
});

app.MapPost("/debug/user/{userId}", async (
    string userId,
    string message,
    IHubContext<ProjectRoomHub> hubContext) =>
{
    await hubContext.Clients.User(userId).SendAsync("ReceivePrivateMessage", userId, message);
    return Results.Ok(new { sent = true, userId, message, note = "需要已配置 UserIdentifier 才能真正命中连接" });
});

app.MapHub<ProjectRoomHub>("/hubs/project-room");

app.Run();

public sealed class ProjectRoomHub : Hub
{
    public async Task JoinProject(string projectCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, projectCode);
        await Clients.Caller.SendAsync("ReceiveSystemMessage", $"joined:{projectCode}");
    }

    public async Task LeaveProject(string projectCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectCode);
        await Clients.Caller.SendAsync("ReceiveSystemMessage", $"left:{projectCode}");
    }

    public async Task SendToProject(string projectCode, string user, string message)
    {
        await Clients.Group(projectCode).SendAsync("ReceiveGroupMessage", user, message);
    }
}
