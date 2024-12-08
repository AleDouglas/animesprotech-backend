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
    public class AnimesController : ControllerBase{
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public AnimesController(AppDbContext context, LogService logService){
            _context = context;
            _logService = logService;
        }

        // Método para retornar animes com paginação e filtros
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Anime>>> GetAnimes(
            [FromQuery] string? diretor,
            [FromQuery] string? nome,
            [FromQuery] string? resumo,
            [FromQuery] int pageIndex = 0,  // Definindo valores padrão
            [FromQuery] int pageSize = 10  // Definindo valores padrão
        )
        {
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

        [HttpGet("active")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Anime>>> GetActiveAnimes(){
            return await _context.Animes
                .Where(a => !a.IsDeleted)
                .ToListAsync();
        }

        [HttpGet("desactive")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Anime>>> GetDeletedAnimes(){
            return await _context.Animes
                .Where(a => a.IsDeleted)
                .ToListAsync();
        }

        

        // Método para adicionar um anime
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Anime>> AddAnime(Anime anime){
            if (anime == null)
                return BadRequest("Anime não pode ser nulo.");
            if (string.IsNullOrWhiteSpace(anime.Nome))
                return BadRequest("O nome do anime é obrigatório.");
            if (string.IsNullOrWhiteSpace(anime.Diretor))
                return BadRequest("O diretor do anime é obrigatório.");

            _context.Animes.Add(anime);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                message: $"Anime '{anime.Nome}' #ID: {anime.Id} foi criado com sucesso.",
                level: "Info",
                action: "Create"
            );


            return CreatedAtAction(nameof(GetAnimes), new { id = anime.Id }, anime);
        }

        // Método para editar um anime
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditAnime(int id, Anime updatedAnime){
            if (id != updatedAnime.Id)
                return BadRequest("ID do anime não corresponde ao parâmetro.");

            var anime = await _context.Animes.FindAsync(id);
            if (anime == null)
                return NotFound();
            if (string.IsNullOrWhiteSpace(updatedAnime.Nome))
                return BadRequest("O nome do anime é obrigatório.");
            if (string.IsNullOrWhiteSpace(updatedAnime.Diretor))
                return BadRequest("O diretor do anime é obrigatório.");

            anime.Nome = updatedAnime.Nome;
            anime.Resumo = updatedAnime.Resumo;
            anime.Diretor = updatedAnime.Diretor;

            _context.Animes.Update(anime);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                message: $"Anime '{anime.Nome}' #ID: {anime.Id} foi atualizado com sucesso.",
                level: "Info",
                action: "Update"
            );

            return NoContent();
        }

        // Método para desativar um anime
        [HttpPut("{id}/disable")]
        [Authorize]
        public async Task<IActionResult> DisableAnime(int id){
            var anime = await _context.Animes.FindAsync(id);
            if (anime == null)
                return NotFound();

            anime.IsDeleted = true;
            _context.Animes.Update(anime);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                message: $"Anime '{anime.Nome}' #ID: {anime.Id} foi desativado com sucesso.",
                level: "Info",
                action: "Update"
            );

            return NoContent();
        }

        // Método para reativar um anime
        [HttpPut("{id}/enable")]
        [Authorize]
        public async Task<IActionResult> EnableAnime(int id){
            var anime = await _context.Animes.FindAsync(id);
            if (anime == null)
                return NotFound();

            anime.IsDeleted = false;
            _context.Animes.Update(anime);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                message: $"Anime '{anime.Nome}' #ID: {anime.Id} foi reativado com sucesso.",
                level: "Info",
                action: "Update"
            );

            return NoContent();
        }

        // Método para deletar ( Excluir ) um anime
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnime(int id){
            var anime = await _context.Animes.FindAsync(id);
            if (anime == null)
                return NotFound();

            _context.Animes.Remove(anime);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                message: $"Anime '{anime.Nome}' #ID: {anime.Id} foi excluído com sucesso.",
                level: "Info",
                action: "Delete"
            );

            return NoContent();
        }


    }
}
