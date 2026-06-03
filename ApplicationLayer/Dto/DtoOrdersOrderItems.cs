namespace ApplicationLayer.Dto
{
    public class DtoOrdersOrderItems
    {
        public DtoOrder Order { get; set; } = null!;
        public IEnumerable<DtoOrderItem> OrderItems { get; set; } = Enumerable.Empty<DtoOrderItem>();
    }
}
