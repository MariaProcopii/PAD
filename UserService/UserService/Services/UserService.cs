using Microsoft.AspNetCore.Identity;
using UserService.Utils;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }   

        public async Task<IdentityUser?> GetUserById(string id)
        {
            Console.WriteLine(id);
            return await _userManager.FindByIdAsync(id);
        }
        
        public async Task<IdentityResult> UpdateUserInfo(string id, string newUsername, string newEmail, string? newPassword = null)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        
            if (!string.IsNullOrWhiteSpace(newUsername))
            {
                var usernameResult = await _userManager.SetUserNameAsync(user, newUsername);
                if (!usernameResult.Succeeded)
                {
                    return usernameResult;
                }
            }
        
            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                var emailResult = await _userManager.SetEmailAsync(user, newEmail);
                if (!emailResult.Succeeded)
                {
                    return emailResult;
                }
            }
        
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                var passwordResult = await _userManager.RemovePasswordAsync(user);
                if (!passwordResult.Succeeded)
                {
                    return passwordResult;
                }

                passwordResult = await _userManager.AddPasswordAsync(user, newPassword);
                if (!passwordResult.Succeeded)
                {
                    return passwordResult;
                }
            }
        
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.DeleteAsync(user);
        }
        
        public async Task<IdentityResult> RestoreUser(string id, string email, string username)
        {
            var user = new IdentityUser
            {
                Id = id,
                Email = email,
                UserName = username,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = username.ToUpper(),
                EmailConfirmed = true
            };

            return await _userManager.CreateAsync(user);
        }
    }
}