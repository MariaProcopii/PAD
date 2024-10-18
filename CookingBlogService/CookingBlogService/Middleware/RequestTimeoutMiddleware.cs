using System.Text;

namespace CookingBlogService.Middleware;

public class RequestTimeoutMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TimeSpan _timeout;

    public RequestTimeoutMiddleware(RequestDelegate next, TimeSpan timeout)
    {
        _next = next;
        _timeout = timeout;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var cts = new CancellationTokenSource(_timeout);
        var task = _next(context);
        var completedTask = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cts.Token));

        if (completedTask == task || context.WebSockets.IsWebSocketRequest)
        {
            await task;
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
            context.Response.ContentType = "application/json";

            var message = new
            {
                Status = "Error",
                Message = "Request has timed out. Please try again later."
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(message), Encoding.UTF8);
        }
    }
}