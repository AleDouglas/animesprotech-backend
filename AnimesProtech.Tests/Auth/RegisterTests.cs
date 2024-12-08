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
    public class AuthControllerRegisterTests
    {
        private readonly AuthController _authController;
        private readonly AppDbContext _context;

        public AuthControllerRegisterTests(){
            Env.Load();

            Environment.SetEnvironmentVariable("SECRET_KEY", "8bztTbBUsekiK7cJSIyhzRqaj7ZOkC1LY08aJco3Dus=");

            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: "TestDatabaseForRegister").Options;

            _context = new AppDbContext(options);
            _authController = new AuthController(_context);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsOk(){
            // Arrange
            var registerModel = new RegisterModel{
                Username = "newuser",
                Password = "newpassword",
                Email = "newuser@example.com",
                FullName = "New User"
            };

            // Act
            var result = await _authController.Register(registerModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal("Usuário registrado com sucesso.", okResult.Value);
        }

        [Fact]
        public async Task Register_DuplicateUser_ReturnsBadRequest(){
            // Arrange
            var registerModel = new RegisterModel{
                Username = "existinguser",
                Password = "password",
                Email = "existinguser@example.com",
                FullName = "Existing User"
            };

            // Adiciona o usuário ao banco
            await _authController.Register(registerModel);

            // Tenta registrar novamente
            var result = await _authController.Register(registerModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Usuário já existe.", badRequestResult.Value);
        }
    }
}
