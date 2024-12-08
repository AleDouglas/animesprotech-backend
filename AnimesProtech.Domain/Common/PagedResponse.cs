namespace AnimesProtech.Domain.Common
{
    public class PagedResponse<T>
    {
        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; }

        public PagedResponse()
        {
            Items = new List<T>();
        }
    }
}
