namespace DietAssistant.Business.Contracts.Models.Paging
{
    public class PagedResult<T>
    {
        public List<T> Results { get; set; }

        public Int32 Page { get; set; }

        public Int32 PageSize { get; set; }

        public Int32 TotalCount { get; set; }
    }
}
