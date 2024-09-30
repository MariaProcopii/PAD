using Microsoft.AspNetCore.Mvc;

namespace CookingBlogService.Controllers;

[ApiController]
public class StatusController : ControllerBase
{

    [HttpGet("blog/status")]
    public async Task<IActionResult> GetStatus()
    {
        var status = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Message = "Cooking Blog Service health status"
        };

        return Ok(status);
    }
}