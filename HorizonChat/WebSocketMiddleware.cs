using System.Net.WebSockets;
using System.Text;

namespace HorizonChat;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<WebSocketMiddleware> _logger;

    public WebSocketMiddleware(RequestDelegate next, ILogger<WebSocketMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var socket = await context.WebSockets.AcceptWebSocketAsync();
                _logger.LogInformation("WebSocket client connected from {RemoteIpAddress}", context.Connection.RemoteIpAddress);
                var buffer = new byte[1024 * 4];
                
                try
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    while (!result.CloseStatus.HasValue)
                    {
                        // Echo back received text (placeholder for chat logic)
                        var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var outgoing = Encoding.UTF8.GetBytes(text);
                        await socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    }
                    await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                    _logger.LogInformation("WebSocket client disconnected gracefully");
                }
                catch (WebSocketException ex)
                {
                    _logger.LogWarning(ex, "WebSocket connection closed unexpectedly");
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                _logger.LogWarning("Invalid WebSocket request from {RemoteIpAddress}", context.Connection.RemoteIpAddress);
            }
        }
        else
        {
            await _next(context);
        }
    }
}
