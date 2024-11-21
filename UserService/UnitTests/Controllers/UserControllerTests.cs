using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserService.Controllers;
using UserService.DTO;
using UserService.Utils;

namespace UnitTests.Controllers;

    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _userController = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetUserById_UserExists_ReturnsOkResultWithUserInfo()
        {
            var userId = "123";
            var user = new IdentityUser { Id = userId, Email = "test@example.com", UserName = "testuser" };
            _userServiceMock.Setup(x => x.GetUserById(userId)).ReturnsAsync(user);

            var result = await _userController.GetUserById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var userInfo = Assert.IsType<UserInfoDTO>(okResult.Value);
            Assert.Equal("test@example.com", userInfo.Email);
            Assert.Equal("testuser", userInfo.Username);
        }

        [Fact]
        public async Task GetUserById_UserDoesNotExist_ReturnsNotFound()
        {
            var userId = "123";
            _userServiceMock.Setup(x => x.GetUserById(userId)).ReturnsAsync((IdentityUser)null);

            var result = await _userController.GetUserById(userId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteUser_UserDeletedSuccessfully_ReturnsOk()
        {
            var userId = "123";
            _userServiceMock.Setup(x => x.DeleteUser(userId)).ReturnsAsync(IdentityResult.Success);

            var result = await _userController.DeleteUser(userId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_UserDeletionFails_ReturnsBadRequest()
        {
            var userId = "123";
            var identityErrors = new[] { new IdentityError { Description = "Deletion failed" } };
            _userServiceMock.Setup(x => x.DeleteUser(userId)).ReturnsAsync(IdentityResult.Failed(identityErrors));

            var result = await _userController.DeleteUser(userId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(identityErrors, badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateUser_UserUpdatedSuccessfully_ReturnsOk()
        {
            var userId = "123";
            var updateUserDto = new UpdateUserDTO { Username = "newuser", Email = "new@example.com", Password = "newpassword" };
            _userServiceMock.Setup(x => x.UpdateUserInfo(userId, updateUserDto.Username, updateUserDto.Email, updateUserDto.Password))
                            .ReturnsAsync(IdentityResult.Success);

            var result = await _userController.UpdateUser(userId, updateUserDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User information updated successfully.", okResult.Value);
        }

        [Fact]
        public async Task UpdateUser_UserUpdateFails_ReturnsBadRequest()
        {
            var userId = "123";
            var updateUserDto = new UpdateUserDTO { Username = "newuser", Email = "new@example.com", Password = "newpassword" };
            var identityErrors = new[] { new IdentityError { Description = "Update failed" } };
            _userServiceMock.Setup(x => x.UpdateUserInfo(userId, updateUserDto.Username, updateUserDto.Email, updateUserDto.Password))
                            .ReturnsAsync(IdentityResult.Failed(identityErrors));

            var result = await _userController.UpdateUser(userId, updateUserDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(identityErrors, badRequestResult.Value);
        }
    }