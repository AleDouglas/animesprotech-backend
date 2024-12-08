using Microsoft.EntityFrameworkCore;
using AnimesProtech.Domain.Entities;

namespace AnimesProtech.Web.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Anime> Animes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Para mapeamento futuro
        }
    }
}