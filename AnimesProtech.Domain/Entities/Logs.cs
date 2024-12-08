/*
    * Classe de entidade Anime
*/

namespace AnimesProtech.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public required string Message { get; set; }
        public required string Level { get; set; } // ( Info, Debug, Warning, Error, Critical )
        public required string Action { get; set; } // ( Create, Update, Delete )
        public required string Timestamp { get; set; }

    }
}