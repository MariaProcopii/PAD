using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.DTO;

namespace UserService.Controllers
{
    [Route("[controller]/profile")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService.Services.UserService _userService;

        public UserController(UserService.Services.UserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string id)
        {
            await Task.Delay(15000);
            var user = await _userService.GetUserById(id);
            if (user == null) return NotFound("User not found.");
            UserInfoDTO userInfoDto = new UserInfoDTO
            {
                Email = user.Email,
                Username = user.UserName
            };
            return Ok(userInfoDto);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUser(id);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }
        
        [HttpPut("edit/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO model)
        {
            var result = await _userService.UpdateUserInfo(id, model.Username, model.Email, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User information updated successfully.");
        }
    }
}