namespace AnimesProtech.Infrastructure.Services
{
    public interface ILogService
    {
        Task LogAsync(string message, string level, string action);
    }
}
