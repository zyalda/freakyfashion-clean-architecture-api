namespace FreakyFashion.PaginationDTO
{
    public class PagedResponse<T> where T : class
    {
        public IEnumerable<T> EntitiesDto { get; set; } = Enumerable.Empty<T>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        // Helper properties to handle view rendering logic cleanly
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
