using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Entites
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int TheTotal { get; set; } = 0;
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
