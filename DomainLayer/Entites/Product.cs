using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DomainLayer.Entites
{
    public class Product
    {
        public Product()
        {
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }

        public string Image { get; set; }
        public string UrlSlug { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}