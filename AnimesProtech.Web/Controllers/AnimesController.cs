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

        public AnimesController(AppDbContext context){
            _context = context;
        }

        // Método para retornar todos os animes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Anime>>> GetAnimes(){
            return await _context.Animes.ToListAsync();
        }

        // Método para retornar um anime específico
        [HttpPost]
        public async Task<ActionResult<Anime>> AddAnime(Anime anime){
            _context.Animes.Add(anime);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAnimes), new { id = anime.Id }, anime);
        }


        // Método para adicionar um anime
        [HttpPost]
        public async Task<ActionResult<Anime>> AddAnime(Anime anime){
            if (anime == null)
                return BadRequest("Anime não pode ser nulo.");

            _context.Animes.Add(anime);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAnimes), new { id = anime.Id }, anime);
        }

        // Método para editar um anime
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAnime(int id, Anime updatedAnime){
            if (id != updatedAnime.Id)
                return BadRequest("ID do anime não corresponde ao parâmetro.");

            var anime = await _context.Animes.FindAsync(id);
            if (anime == null)
                return NotFound();

            anime.Nome = updatedAnime.Nome;
            anime.Resumo = updatedAnime.Resumo;
            anime.Diretor = updatedAnime.Diretor;
            anime.IsDeleted = updatedAnime.IsDeleted;

            _context.Animes.Update(anime);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método para deletar ( Excluir ) um anime
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnime(int id){
            var anime = await _context.Animes.FindAsync(id);
            if (anime == null)
                return NotFound();

            _context.Animes.Remove(anime);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método para retornar animes com paginação e filtros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Anime>>> GetAnimes(
            [FromQuery] string? diretor,
            [FromQuery] string? nome,
            [FromQuery] string? resumo,
            [FromQuery] int pageIndex = 0,  // Definindo valores padrão
            [FromQuery] int pageSize = 10  // Definindo valores padrão
        ){
            var query = _context.Animes
                .Where(a => !a.IsDeleted) // Apenas animes ativos
                .AsQueryable();

            // Filtros opcionais
            if (!string.IsNullOrEmpty(diretor))
                query = query.Where(a => a.Diretor.Contains(diretor));

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(a => a.Nome.Contains(nome));

            if (!string.IsNullOrEmpty(resumo))
                query = query.Where(a => a.Resumo.Contains(resumo));

            // Paginação
            var totalRecords = await query.CountAsync();
            var animes = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Retorno com paginação
            return Ok(new
            {
                TotalRecords = totalRecords,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Animes = animes
            });
        }
    }
}
