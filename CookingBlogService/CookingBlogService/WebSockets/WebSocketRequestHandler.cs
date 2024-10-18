namespace CookingBlogService.WebSockets;


public static class WebSocketRequestHandler
{
    public static async Task HandleWebSocketRequest(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            Console.WriteLine("WebSocket connection established");

            await WebSocketConnectionManager.HandleConnection(webSocket);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("WebSocket connection expected.");
        }
    }
}