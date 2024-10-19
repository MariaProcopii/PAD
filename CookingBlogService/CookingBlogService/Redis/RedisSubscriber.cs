using CookingBlogService.WebSockets;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace CookingBlogService.Redis;
public class RedisSubscriber
{
    private readonly IConnectionMultiplexer _redis;

    public RedisSubscriber()
    {
        _redis = ConnectionMultiplexer.Connect("my-redis-db");
    }

    public void SubscribeToRoom(string room)
    {
        var subscriber = _redis.GetSubscriber();
        subscriber.Subscribe(room, async (channel, message) =>
        {
            await HandleIncomingMessage(room, message);
        });
    }

    private async Task HandleIncomingMessage(string room, RedisValue message)
    {
        var messageParts = message.ToString().Split(':', 2);
        if (messageParts.Length == 2)
        {
            var senderSocketId = messageParts[0];
            var actualMessage = messageParts[1];

            await WebSocketConnectionManager.SendMessageToRoomAsync(room, actualMessage, senderSocketId);
        }
    }
}