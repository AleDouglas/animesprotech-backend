using Microsoft.AspNetCore.Mvc;
using AnimesProtech.Infrastructure.Data;
using AnimesProtech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnimesProtech.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogsController(AppDbContext context){
            _context = context;
        }

        // Método para retornar todos os animes sem paginação e filtros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Log>>> GetLogs(){
            return await _context.Logs.ToListAsync();
        }


    }
}
