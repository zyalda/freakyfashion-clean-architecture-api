namespace ApplicationLayer.Dto
{
    public class DtoOrdersOrderItems
    {
        public string? OrderNumber { get; set; }
        public DtoCustomer CustomerInfo {  get; set; } = new DtoCustomer();
        public DtoOrder Order { get; set; } = new DtoOrder();
        public IEnumerable<DtoOrderItem> OrderItems { get; set; } = Enumerable.Empty<DtoOrderItem>();
    }
}
