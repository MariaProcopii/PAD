using Microsoft.AspNetCore.Identity;

namespace UserService.Utils;

public interface IUserService
{
    Task<IdentityUser?> GetUserById(string id);
    Task<IdentityResult> UpdateUserInfo(string id, string newUsername, string newEmail, string? newPassword = null);
    Task<IdentityResult> DeleteUser(string id);
    Task<IdentityResult> RestoreUser(string id, string email, string username);
}
