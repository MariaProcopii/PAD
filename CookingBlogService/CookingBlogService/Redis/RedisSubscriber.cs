namespace CookingBlogService.Redis;

using StackExchange.Redis;
using System.Text;
using CookingBlogService.WebSockets;

public class RedisSubscriber
{
    private readonly IConnectionMultiplexer _redis;

    public RedisSubscriber()
    {
        _redis = ConnectionMultiplexer.Connect("localhost");
        Subscribe();
    }

    private void Subscribe()
    {
        var subscriber = _redis.GetSubscriber();
        subscriber.Subscribe("chatroom", (channel, message) =>
        {
            HandleIncomingMessage(message);
        });
    }

    private void HandleIncomingMessage(RedisValue message)
    {
        var messageParts = message.ToString().Split(':', 2);
        if (messageParts.Length == 2)
        {
            var senderSocketId = messageParts[0];
            var actualMessage = messageParts[1];

            WebSocketConnectionManager.SendMessageToAllAsync(actualMessage, senderSocketId).Wait();
        }
    }
}