using FreakyFashionClient.Models;

namespace FreakyFashionClient.PaginationDTO
{
    public class PagedResponse
    {
        public IEnumerable<ProductModel> Products { get; set; } = Enumerable.Empty<ProductModel>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        // Helper properties to handle view rendering logic cleanly
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
