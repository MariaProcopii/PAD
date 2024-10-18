using Microsoft.AspNetCore.Identity;
using Moq;

namespace UnitTests.Services;

public class UserServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly UserService.Services.UserService _userService;

        public UserServiceTests()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            _userService = new UserService.Services.UserService(_userManagerMock.Object);
        }

        [Fact]
        public async Task GetUserById_UserExists_ReturnsUser()
        {
            var userId = "123";
            var user = new IdentityUser { Id = userId };
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            var result = await _userService.GetUserById(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            _userManagerMock.Verify(x => x.FindByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserById_UserDoesNotExist_ReturnsNull()
        {
            var userId = "123";
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((IdentityUser)null);

            var result = await _userService.GetUserById(userId);

            Assert.Null(result);
            _userManagerMock.Verify(x => x.FindByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInfo_UserNotFound_ReturnsFailure()
        {
            var userId = "123";
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((IdentityUser)null);

            var result = await _userService.UpdateUserInfo(userId, "newUsername", "newEmail");

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, error => error.Description == "User not found");
            _userManagerMock.Verify(x => x.FindByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInfo_UpdatesUsernameAndEmail_Success()
        {
            var userId = "123";
            var user = new IdentityUser { Id = userId };
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.SetUserNameAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.SetEmailAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var result = await _userService.UpdateUserInfo(userId, "newUsername", "newEmail");

            Assert.True(result.Succeeded);
            _userManagerMock.Verify(x => x.SetUserNameAsync(user, "newUsername"), Times.Once);
            _userManagerMock.Verify(x => x.SetEmailAsync(user, "newEmail"), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInfo_FailsOnPasswordUpdate_ReturnsFailure()
        {
            var userId = "123";
            var user = new IdentityUser { Id = userId };
    
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.SetUserNameAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.SetEmailAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.RemovePasswordAsync(user)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var result = await _userService.UpdateUserInfo(userId, "newUsername", "newEmail", "newPassword");

            Assert.False(result.Succeeded);
            _userManagerMock.Verify(x => x.RemovePasswordAsync(user), Times.Once);
            _userManagerMock.Verify(x => x.AddPasswordAsync(user, "newPassword"), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_UserExists_ReturnsSuccess()
        {
            var userId = "123";
            var user = new IdentityUser { Id = userId };
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _userService.DeleteUser(userId);

            Assert.True(result.Succeeded);
            _userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsFailure()
        {
            var userId = "123";
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((IdentityUser)null);

            var result = await _userService.DeleteUser(userId);

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, error => error.Description == "User not found");
            _userManagerMock.Verify(x => x.FindByIdAsync(userId), Times.Once);
        }
    }