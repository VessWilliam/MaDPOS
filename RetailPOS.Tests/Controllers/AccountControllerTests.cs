using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Controllers;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using System.Threading.Tasks;
using Xunit;

namespace RetailPOS.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            // Note: In a real test, you would need to mock UserManager and SignInManager
            // This is a simplified version for demonstration
            _controller = new AccountController(_userManager, _signInManager);
        }

        [Fact]
        public async Task Register_WithValidData_RedirectsToHome()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            // Act
            var result = await _controller.Register(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task Register_WithInvalidData_ReturnsViewWithModel()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "invalid-email",
                Password = "short",
                ConfirmPassword = "short"
            };
            _controller.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = await _controller.Register(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Login_WithValidCredentials_RedirectsToHome()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Test123!",
                RememberMe = false
            };

            // Act
            var result = await _controller.Login(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsViewWithModel()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "WrongPassword",
                RememberMe = false
            };

            // Act
            var result = await _controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.True(_controller.ModelState.ErrorCount > 0);
        }
    }
}