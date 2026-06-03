using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Entites
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string PassWord { get; set; } = string.Empty;

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
