namespace FreakyFashionClient.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; }
        public string UrlSlug { get; set; }
    }
}
