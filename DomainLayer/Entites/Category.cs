using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Entites
{
    public class Category
    {
        public Category()
        {
            Products = new List<Product>();
        }


        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; }
        public string UrlSlug { get; set; }
        public List<Product> Products { get; set; }
    }
}
