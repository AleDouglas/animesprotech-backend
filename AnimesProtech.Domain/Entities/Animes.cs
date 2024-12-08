/*
    * Classe de entidade Anime
*/

namespace MinhaApi.Domain.Entities
{
    public class Anime
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Resumo { get; set; }
        public string Diretor { get; set; }
        public bool IsDeleted { get; set; }
    }
}