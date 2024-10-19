using CookingBlogService.Redis;
using CookingBlogService.WebSockets;
using Microsoft.AspNetCore.Mvc;

namespace CookingBlogService.Controllers;

[ApiController]
[Route("ws")]
public class WebSocketController : ControllerBase
{
    private readonly RedisSubscriber _redisSubscriber;

    public WebSocketController(RedisSubscriber redisSubscriber)
    {
        _redisSubscriber = redisSubscriber;
    }

    [HttpGet("connect")]
    public async Task Connect()
    {
        await WebSocketRequestHandler.HandleWebSocketRequest(HttpContext, _redisSubscriber);
    }
}