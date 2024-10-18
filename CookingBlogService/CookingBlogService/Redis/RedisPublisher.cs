namespace CookingBlogService.Redis;

using StackExchange.Redis;

public static class RedisPublisher
{
    private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

    public static async Task PublishMessage(string message, string senderSocketId)
    {
        var pubsub = redis.GetSubscriber();
        var messageWithSender = $"{senderSocketId}:{message}";

        await pubsub.PublishAsync("chatroom", messageWithSender);
    }
}