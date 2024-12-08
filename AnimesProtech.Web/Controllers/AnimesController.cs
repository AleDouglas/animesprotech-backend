using Microsoft.AspNetCore.Mvc;
using AnimesProtech.Infrastructure.Data;
using AnimesProtech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnimesProtech.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnimesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Anime>>> GetAnimes()
        {
            return await _context.Animes.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Anime>> AddAnime(Anime anime)
        {
            _context.Animes.Add(anime);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAnimes), new { id = anime.Id }, anime);
        }
    }
}
