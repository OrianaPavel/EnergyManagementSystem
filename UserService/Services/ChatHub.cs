using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    private static string adminConnection;
    private static Dictionary<string, string> userConnections = new Dictionary<string, string>();
    private readonly ILogger<ChatHub> _logger;
    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    /*
    public async Task SendMessage(string userId,string message)
    {
        
    await Clients.All.SendAsync("receivedMessage", userId, message);
    }*/
    public async Task SendMessage(string userType, string userId, string message)
    {
        //var userType = Context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (userType == "user")
        {
            if (adminConnection != null)
            {
                await Clients.Client(adminConnection).SendAsync("receivedMessage", userType,userId, message);
            }
            else
            {
                await Clients.Caller.SendAsync("NoAdminOnline", "Sorry the admin is not available.Try again later!");
            }
        }

        else if (userType == "admin" && userConnections.TryGetValue(userId, out var userConnection))
        {
            _logger.LogInformation("User is admin and userId is:" + userId + " and connection is:" + userConnection);
            await Clients.Client(userConnection).SendAsync("receivedMessage", userType, userId, message);
        }
    }
    public async Task Typing(string userId,string userType){
        string message = userType + " is typing!";
        if(userType == "user" && adminConnection != null){
            await Clients.Client(adminConnection).SendAsync("TypingNotification",message);
        }
        else if (userType == "admin" && userConnections.TryGetValue(userId, out var userConnection))
        {
            _logger.LogInformation("User is admin and userId is:" + userId + " and connection is:" + userConnection);
            await Clients.Client(userConnection).SendAsync("TypingNotification",message);
        }
    }
    public override async Task OnConnectedAsync()
    {
        /*
        var userType = Context.User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userType == "admin")
        {
            adminConnection = Context.ConnectionId;
        }
        else
        {
            userConnections[userId] = Context.ConnectionId;
        }*/
        _logger.LogInformation($"Connection {Context.ConnectionId} connected.");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        /*var userType = Context.User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userType == "admin")
        {
            adminConnection = null;
        }
        else
        {
            userConnections.Remove(userId);
        }*/
        _logger.LogInformation($"Connection {Context.ConnectionId} disconnected.");
        await base.OnDisconnectedAsync(exception);
    }
     public async Task SendUserTypeAndUserId(string userRole, string userId)
    {
        if (userRole == "admin")
        {
            adminConnection = Context.ConnectionId;
        }
        else
        {
            userConnections[userId] = Context.ConnectionId;
        }
        
    }
}
