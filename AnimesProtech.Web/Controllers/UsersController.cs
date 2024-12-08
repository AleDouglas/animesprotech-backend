using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AnimesProtech.Infrastructure.Data;
using AnimesProtech.Infrastructure.Services;
using AnimesProtech.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace AnimesProtech.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public UsersController(AppDbContext context, LogService logService){
            _context = context;
            _logService = logService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            [FromQuery] string? Username,
            [FromQuery] string? FullName,
            [FromQuery] string? Email,
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 10)
        {

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(Username))
                query = query.Where(u => u.Username.Contains(Username));

            if (!string.IsNullOrEmpty(FullName))
                query = query.Where(u => u.FullName.Contains(FullName));
            
            if (!string.IsNullOrEmpty(Email))
                query = query.Where(u => u.Email.Contains(Email));

            var totalRecords = await query.CountAsync();
            var users = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new{
                TotalRecords = totalRecords,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Users = users
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id){
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> CreateUser(User user){
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.PasswordHash))
                return BadRequest("Username e senha são obrigatórios.");

            // Verifica se o usuário já existe
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest("Usuário já existe.");

            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                message: $"Usuário {user.Username} #{user.Id} criado.",
                level: "Info",
                action: "Create"
            );

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser){
            if (id != updatedUser.Id)
                return BadRequest("ID do usuário não corresponde ao parâmetro.");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.Role = updatedUser.Role;
            user.IsActive = updatedUser.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                message: $"Usuário {user.Username} #{user.Id} atualizado.",
                level: "Info",
                action: "Update"
            );

            return NoContent();
        }

        [HttpPut("{id}/disable")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DisableUser(int id){
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            user.IsActive = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                message: $"Usuário {user.Username} #{user.Id} desativado.",
                level: "Info",
                action: "Disable"
            );

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteUser(int id){
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                message: $"Usuário {user.Username} #{user.Id} excluído.",
                level: "Info",
                action: "Delete"
            );

            return NoContent();
        }


    }
}
