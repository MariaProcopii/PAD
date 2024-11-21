using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.DTO;
using UserService.Utils;

namespace UserService.Controllers
{
    [Route("[controller]/profile")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet("info/{id}")]
        // [Authorize]
        public async Task<IActionResult> GetUserById(string id)
        {
            // await Task.Delay(15000);
            var user = await _userService.GetUserById(id);
            if (user == null) return NotFound("User not found.");
            UserInfoDTO userInfoDto = new UserInfoDTO
            {
                Email = user.Email,
                Username = user.UserName,
                Password = user.PasswordHash
            };
            return Ok(userInfoDto);
        }

        [HttpDelete("delete/{id}")]
        // [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUser(id);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(id);
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
        
        [HttpPost("restore")]
        public async Task<IActionResult> RestoreUser([FromBody] UserRestoreDTO restoreDTO)
        {
            var result = await _userService.RestoreUser(restoreDTO.Id, restoreDTO.Email, restoreDTO.Username);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(restoreDTO.Id);
        }
    }
}