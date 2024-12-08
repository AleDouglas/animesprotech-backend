/*
    * Classe de entidade Anime
*/

namespace AnimesProtech.Domain.Entities
{
    public class Anime
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Resumo { get; set; }
        public required string Diretor { get; set; }
        public bool IsDeleted { get; set; }
    }
}