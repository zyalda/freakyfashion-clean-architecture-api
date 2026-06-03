namespace FreakyFashionClient.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Image { get; set; }
        public IFormFile ImageFile { get; set; }
        public string UrlSlug { get; set; }
        public string Category { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
    }
}
