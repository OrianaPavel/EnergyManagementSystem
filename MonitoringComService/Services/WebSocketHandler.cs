using System.Net.WebSockets;
using System.Text;

public class WebSocketHandler
{
    private readonly Dictionary<string, WebSocket> _connections = new Dictionary<string, WebSocket>();
    private readonly ILogger<WebSocketHandler> _logger;

    public WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        _logger = logger;
    }

    public void HandleConnection(string userId, WebSocket webSocket)
    {
        _connections[userId] = webSocket;
        _logger.LogInformation($"New WebSocket connection established for user {userId}.");
    }

    public async Task SendMessageAsync(string userId, string message)
    {
        if (_connections.TryGetValue(userId, out var webSocket) && webSocket.State == WebSocketState.Open)
        {
            try
            {
                var messageBuffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(messageBuffer);
                await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                _logger.LogInformation($"Message sent to user {userId}: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message to user {userId}.");
            }
        }
        else
        {
            _logger.LogWarning($"Attempted to send message to user {userId}, but the WebSocket is closed or not found.");
        }
    }
}
