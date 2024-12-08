using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetEnv;
using AnimesProtech.Infrastructure.Data;
using AnimesProtech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnimesProtech.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context){
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model){
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == model.Username);

            if (user == null || !VerifyPasswordHash(model.Password, user.PasswordHash))
                return Unauthorized(new { Message = "Credenciais inválidas." });

            if (!user.IsActive)
                return Unauthorized(new { Message = "Usuário inativo." });

            var secretKey = Env.GetString("SECRET_KEY") ?? throw new InvalidOperationException("SECRET_KEY não configurada no .env");
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(new[]{
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new{
                Token = tokenHandler.WriteToken(token),
                Expires = tokenDescriptor.Expires
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model){
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                return BadRequest("Usuário já existe.");

            CreatePasswordHash(model.Password, out string passwordHash);

            var user = new User{
                Username = model.Username,
                PasswordHash = passwordHash,
                Email = model.Email,
                FullName = model.FullName,
                Role = "User", // Padrão para novos usuários
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuário registrado com sucesso.");
        }

        private void CreatePasswordHash(string password, out string passwordHash){
            passwordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, string storedHash){
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password)) == storedHash;
        }
    }

    public class LoginModel{
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterModel{
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
    }
}
