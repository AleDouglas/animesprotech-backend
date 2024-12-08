using AnimesProtech.Domain.Entities;
using AnimesProtech.Infrastructure.Data;
using System.Threading.Tasks;

namespace AnimesProtech.Infrastructure.Services
{
    public class LogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string message, string level, string action)
        {
            var log = new Log
            {
                Message = message,
                Level = level,
                Action = action,
                Timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format
            };

            _context.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
