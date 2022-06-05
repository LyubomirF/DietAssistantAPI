namespace DietAssistant.Business.Contracts.Models.Paging
{
    public class PagingParameters
    {
        private const int MAX_PAGE_SIZE = 20;
        private const int DEFAULT_PAGE = 1;

        private int _pageSize = MAX_PAGE_SIZE;
        private int _page = DEFAULT_PAGE;


        public int Page
        {
            get => _page;
            set => _page = (value <= 0) ? DEFAULT_PAGE : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MAX_PAGE_SIZE || value <= 0) ? MAX_PAGE_SIZE : value;
        }
    }
}
