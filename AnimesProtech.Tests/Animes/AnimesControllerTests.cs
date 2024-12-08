using AnimesProtech.Domain.Entities;
using AnimesProtech.Infrastructure.Data;
using AnimesProtech.Infrastructure.Services;
using AnimesProtech.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using DotNetEnv;
using AnimesProtech.Domain.Common;

namespace AnimesProtech.Tests.Controllers
{
    public class AnimesControllerTests{
        private readonly AuthController _authController;
        private readonly AnimesController _controller;
        private readonly AppDbContext _context;
        private readonly Mock<ILogService> _mockLogService;
        

        public AnimesControllerTests(){
            Env.Load();

            Environment.SetEnvironmentVariable("SECRET_KEY", "8bztTbBUsekiK7cJSIyhzRqaj7ZOkC1LY08aJco3Dus=");

            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options;

            _context = new AppDbContext(options);
            _authController = new AuthController(_context);
            _mockLogService = new Mock<ILogService>();
            _mockLogService.Setup(log => log.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _context.Animes.RemoveRange(_context.Animes);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            _context.Animes.Add(new Anime { Id = 1, Nome = "Naruto", Diretor = "Hayato Date", Resumo = "A história de Naruto.", IsDeleted = false });
            _context.Animes.Add(new Anime { Id = 2, Nome = "Dragon Ball", Diretor = "Akira Toriyama", Resumo = "A jornada de Goku.", IsDeleted = false });
            _context.Animes.Add(new Anime { Id = 3, Nome = "One Piece", Diretor = "Eiichiro Oda", Resumo = "A aventura de Luffy.", IsDeleted = true });
            _context.SaveChanges();

            _controller = new AnimesController(_context, _mockLogService.Object);
        }

        private async Task<string> AuthenticateAsAdmin(){
            _context.Users.Add(new User
            {
                Id = 99,
                Username = "adminuser",
                PasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes("adminpassword")),
                Role = "Admin",
                FullName = "Admin User",
                Email = "teste@gmail.com",
                IsActive = true
            });
            await _context.SaveChangesAsync();

            var loginModel = new LoginModel
            {
                Username = "adminuser",
                Password = "adminpassword"
            };

            var result = await _authController.Login(loginModel);

            var okResult = Assert.IsType<OkObjectResult>(result); 
            Assert.NotNull(okResult.Value);

            var tokenProperty = okResult.Value.GetType().GetProperty("Token");
            Assert.NotNull(tokenProperty);

            var token = tokenProperty.GetValue(okResult.Value) as string;
            Assert.NotNull(token);

            return token;
        }

        [Fact]
        public async Task GetAnimes_ReturnsPagedAndFilteredResults(){
            var result = await _controller.GetAnimes(nome: "Naruto", diretor: null, resumo: null, pageIndex: 0, pageSize: 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PagedResponse<Anime>>(okResult.Value);

            Assert.Equal(1, response.TotalRecords);
            Assert.Single(response.Items);
            Assert.Equal("Naruto", response.Items[0].Nome);
        }

        [Fact]
        public async Task GetAnimeActive_ReturnsOnlyActiveAnimes(){
            // Act
            var result = await _controller.GetActiveAnimes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // Acessa o .Result
            var response = Assert.IsType<List<Anime>>(okResult.Value);   // Valida o conteúdo

            Assert.Equal(2, response.Count);

            Assert.DoesNotContain(response, a => a.IsDeleted);
        }

        [Fact]
        public async Task GetAnimeDeleted_ReturnsOnlyDeletedAnimes(){
            // Act
            var result = await _controller.GetDeletedAnimes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // Acessa o .Result
            var response = Assert.IsType<List<Anime>>(okResult.Value);   // Valida o conteúdo

            Assert.Single(response);

            Assert.Contains(response, a => a.IsDeleted);
        }

        [Fact]
        public async Task AddAnime_ReturnsCreatedResult(){
            // Authenticate as admin
            var token = await AuthenticateAsAdmin();

            // Adiciona o token no cabeçalho de autorização
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
                    {
                        Headers = { ["Authorization"] = $"Bearer {token}" }
                    }
                }
            };

            // Arrange
            var newAnime = new Anime
            {
                Nome = "Bleach",
                Diretor = "Tite Kubo",
                Resumo = "A história de Ichigo Kurosaki."
            };

            // Act
            var result = await _controller.AddAnime(newAnime);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<Anime>(createdResult.Value);

            Assert.Equal(newAnime.Nome, response.Nome);
            Assert.Equal(newAnime.Diretor, response.Diretor);
            Assert.Equal(newAnime.Resumo, response.Resumo);
        }

        [Fact]
        public async Task UpdateAnime_ReturnsNoContentResult(){
            var token = await AuthenticateAsAdmin();

            _controller.ControllerContext = new ControllerContext{
                HttpContext = new DefaultHttpContext{
                    Request = {
                        Headers = { ["Authorization"] = $"Bearer {token}" }
                    }
                }
            };

            var updatedAnime = new Anime{
                Id = 1,
                Nome = "Naruto Shippuden",
                Diretor = "Hayato Date",
                Resumo = "A continuação da história de Naruto."
            };

            var result = await _controller.EditAnime(updatedAnime.Id, updatedAnime);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAnime_ReturnsNoContentResult(){
            var token = await AuthenticateAsAdmin();

            _controller.ControllerContext = new ControllerContext{
                HttpContext = new DefaultHttpContext{
                    Request = {
                        Headers = { ["Authorization"] = $"Bearer {token}" }
                    }
                }
            };

            var result = await _controller.DeleteAnime(1);

            Assert.IsType<NoContentResult>(result);
        }
       
    }
}
