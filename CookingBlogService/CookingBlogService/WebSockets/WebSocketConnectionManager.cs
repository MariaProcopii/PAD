using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace CookingBlogService.WebSockets;

public static class WebSocketConnectionManager
    {
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> _rooms = new ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>>();

        public static string AddSocketToRoom(string room, WebSocket socket)
        {
            var socketId = Guid.NewGuid().ToString();
            if (!_rooms.ContainsKey(room))
            {
                _rooms[room] = new ConcurrentDictionary<string, WebSocket>();
            }
            _rooms[room].TryAdd(socketId, socket);
            return socketId;
        }

        public static async Task RemoveSocketFromRoom(string room, string socketId)
        {
            if (_rooms.ContainsKey(room))
            {
                _rooms[room].TryRemove(socketId, out var socket);
                if (socket != null && socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the server", CancellationToken.None);
                }
            }
        }

        public static async Task SendMessageToRoomAsync(string room, string message, string senderSocketId)
        {
            if (_rooms.ContainsKey(room))
            {
                foreach (var socketEntry in _rooms[room])
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
    }