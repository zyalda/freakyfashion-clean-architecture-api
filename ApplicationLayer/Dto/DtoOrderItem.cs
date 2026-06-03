using DomainLayer.Entites;

namespace ApplicationLayer.Dto
{
    public class DtoOrderItem
    {
        public int Quantity { get; set; }

        public int UnitPrice { get; set; }

        public int OrderId { get; set; }
        public DtoOrder Order { get; set; } = null!;

        public int ProductId { get; set; }
    }
}
