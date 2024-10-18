using System.Net.WebSockets;
using System.Text;
using CookingBlogService.Redis;
using Microsoft.AspNetCore.SignalR;

namespace CookingBlogService.WebSockets;


public static class WebSocketRequestHandler
{
    public static async Task HandleWebSocketRequest(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socketId = WebSocketConnectionManager.AddSocket(webSocket);
            Console.WriteLine($"WebSocket connection established with ID: {socketId}");

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            // Receive messages from the client and broadcast to all other clients
            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received message from {socketId}: {message}");

                // Publish the message to Redis
                await RedisPublisher.PublishMessage(message, socketId);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            // Remove the socket when connection is closed
            await WebSocketConnectionManager.RemoveSocket(socketId);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("WebSocket connection expected.");
        }
    }
}