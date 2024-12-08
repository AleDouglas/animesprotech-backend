using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AnimesProtech.Infrastructure.Data;
using AnimesProtech.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace AnimesProtech.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogsController(AppDbContext context){
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Log>>> GetLogs(){
            return await _context.Logs.ToListAsync();
        }


    }
}
