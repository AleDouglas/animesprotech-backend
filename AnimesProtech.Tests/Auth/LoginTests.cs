using AnimesProtech.Web.Controllers;
using AnimesProtech.Domain.Entities;
using AnimesProtech.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;
using DotNetEnv;

namespace AnimesProtech.Tests
{
    public class AuthControllerTests{
        private readonly AuthController _authController;
        private readonly AppDbContext _context;

        public AuthControllerTests(){

            Env.Load();

            Environment.SetEnvironmentVariable("SECRET_KEY", "8bztTbBUsekiK7cJSIyhzRqaj7ZOkC1LY08aJco3Dus=");

            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options;

            _context = new AppDbContext(options);

            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            _context.Users.Add(new User{
                Id = 1,
                Username = "testuser",
                PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("password")), // Simula um hash básico
                FullName = "Test User",
                Email = "teste@gmail.com",
                Role = "User",
                IsActive = true
            });
            _context.SaveChanges();

            _authController = new AuthController(_context);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResult(){
            // Arrange
            var loginModel = new LoginModel{
                Username = "testuser",
                Password = "password"
            };

            // Act
            var result = await _authController.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); 
            Assert.NotNull(okResult.Value);

            // Verifica o retorno esperado do controlador
            var response = okResult.Value.GetType().GetProperty("Token");
            Assert.NotNull(response); 
        }


        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized(){
            // Arrange
            var loginModel = new LoginModel{
                Username = "wronguser",
                Password = "wrongpassword"
            };

            // Act
            var result = await _authController.Login(loginModel);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
