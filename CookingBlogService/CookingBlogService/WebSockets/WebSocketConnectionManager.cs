using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace CookingBlogService.WebSockets;

public static class WebSocketConnectionManager
{
    private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

    public static string AddSocket(WebSocket socket)
    {
        var socketId = Guid.NewGuid().ToString();
        _sockets.TryAdd(socketId, socket);
        return socketId;
    }

    public static async Task RemoveSocket(string socketId)
    {
        _sockets.TryRemove(socketId, out var socket);
        if (socket != null && socket.State == WebSocketState.Open)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the server", CancellationToken.None);
        }
    }

    public static async Task SendMessageToAllAsync(string message, string senderSocketId)
    {
        foreach (var socketEntry in _sockets)
        {
            var socketId = socketEntry.Key;
            var socket = socketEntry.Value;

            if (socketId != senderSocketId && socket.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}