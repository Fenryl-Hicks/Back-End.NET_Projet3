using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
                _mockUserManager.Object, new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object, null, null, null, null);

            _accountController = new AccountController(_mockUserManager.Object, _mockSignInManager.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ShouldRedirectToAdmin()
        {
            // Arrange
            var loginModel = new LoginModel { Name = "testuser", Password = "correctpassword", ReturnUrl = "/Admin/Index" };
            var user = new IdentityUser { UserName = "testuser" };

            _mockUserManager.Setup(u => u.FindByNameAsync("testuser")).ReturnsAsync(user);
            _mockSignInManager.Setup(s => s.PasswordSignInAsync(user, "correctpassword", false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _accountController.Login(loginModel) as RedirectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("/Admin/Index", result.Url);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ShouldReturnViewWithError()
        {
            // Arrange
            var loginModel = new LoginModel { Name = "invaliduser", Password = "wrongpassword" };
            _mockUserManager.Setup(u => u.FindByNameAsync("invaliduser")).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _accountController.Login(loginModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.True(result.ViewData.ModelState.ContainsKey(""));
        }
    }
}
