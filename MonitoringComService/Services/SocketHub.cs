 using Microsoft.AspNetCore.SignalR;
public sealed class SocketHub : Hub
{
    public async Task SubscribeToUserTopic(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        // Example: Send a test message to the connected user
        await Clients.Group(userId).SendAsync("ReceiveMessage", "Test message from server");
        await Clients.Caller.SendAsync("ReceiveMessage", "Test message from server");

    }
    public async Task UnsubscribeFromUserTopic(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
    }

    public async Task SendMessageToUser(string userId, string message)
    {
        await Clients.Group(userId).SendAsync("ReceiveMessage", message);
    }
}