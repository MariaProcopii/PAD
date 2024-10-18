using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers;

[ApiController]
public class StatusController : ControllerBase
{

    [HttpGet("user/status")]
    public async Task<IActionResult> GetStatus()
    {
        //await Task.Delay(4000);
        var status = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Message = "User Service health status"
        };

        return Ok(status);
    }
}