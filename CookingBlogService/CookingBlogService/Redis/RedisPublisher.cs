namespace CookingBlogService.Redis;

using StackExchange.Redis;

public static class RedisPublisher
{
    private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("my-redis-db");

    public static async Task PublishMessage(string room, string message, string senderSocketId)
    {
        var pubsub = redis.GetSubscriber();
        var messageWithSender = $"{senderSocketId}:{message}";

        await pubsub.PublishAsync(room, messageWithSender);
    }
}