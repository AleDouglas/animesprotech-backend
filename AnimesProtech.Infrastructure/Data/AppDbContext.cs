using Microsoft.EntityFrameworkCore;
using AnimesProtech.Domain.Entities;

namespace AnimesProtech.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<Anime> Animes { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Para mapeamento futuro
        }
    }
}