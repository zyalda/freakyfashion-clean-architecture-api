using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Entites
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int UnitPrice { get; set; }

        // Relational links
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        // FIXED: Single product link 💄
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

    }
}
