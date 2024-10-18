using CookingBlogService.WebSockets;
using Microsoft.AspNetCore.Mvc;

namespace CookingBlogService.Controllers;

[ApiController]
[Route("ws")]
public class WebSocketController : ControllerBase
{
    [HttpGet("connect")]
    public async Task Connect()
    {
        await WebSocketRequestHandler.HandleWebSocketRequest(HttpContext);
    }
}