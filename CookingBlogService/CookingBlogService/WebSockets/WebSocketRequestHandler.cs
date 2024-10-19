using System.Net.WebSockets;
using System.Text;
using CookingBlogService.Redis;

namespace CookingBlogService.WebSockets;


public static class WebSocketRequestHandler
    {
        public static async Task HandleWebSocketRequest(HttpContext context, RedisSubscriber redisSubscriber)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var room = context.Request.Query["room"];
                if (string.IsNullOrEmpty(room))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Room parameter is required.");
                    return;
                }

                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var socketId = WebSocketConnectionManager.AddSocketToRoom(room, webSocket);
                Console.WriteLine($"WebSocket connection established with ID: {socketId} in room: {room}");

                redisSubscriber.SubscribeToRoom(room);

                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message from {socketId} in room {room}: {message}");

                    await RedisPublisher.PublishMessage(room, message, socketId);

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                await WebSocketConnectionManager.RemoveSocketFromRoom(room, socketId);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("WebSocket connection expected.");
            }
        }
    }