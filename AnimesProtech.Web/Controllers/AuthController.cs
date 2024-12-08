using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetEnv;

namespace AnimesProtech.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model.Username != "admin" || model.Password != "password")
                return Unauthorized("Usuário ou senha inválidos.");

            var secretKey = Env.GetString("SECRET_KEY") ?? throw new InvalidOperationException("SECRET_KEY não configurada no .env");
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(new[]{
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }

    public class LoginModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
