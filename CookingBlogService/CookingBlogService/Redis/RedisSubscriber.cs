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
        Subscribe();
    }

    private void Subscribe()
    {
        var subscriber = _redis.GetSubscriber();
        subscriber.Subscribe("chatroom", async (channel, message) =>
        {
            await HandleIncomingMessage(message);
        });
    }

    private async Task HandleIncomingMessage(RedisValue message)
    {
        var messageParts = message.ToString().Split(':', 2);
        if (messageParts.Length == 2)
        {
            var senderSocketId = messageParts[0];
            var actualMessage = messageParts[1];

            await WebSocketConnectionManager.SendMessageToAllAsync(actualMessage, senderSocketId);
        }
    }
}