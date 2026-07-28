using ApplicationLayer.Dto;

namespace ApplicationLayer.IServices
{
    public interface IOrderService
    {
        public Task<DtoOrdersOrderItems> AddOrderItemToCart(OrderRequest orderRequest, int customerId);
        public Task<DtoOrdersOrderItems> AddOrder(OrderRequest orderRequest, int customerId);
        public IEnumerable<DtoOrdersOrderItems> GetOrders();
        public Task<DtoOrdersOrderItems> GetOrderById(int id);
        public Task<IEnumerable<DtoOrderListItem>> GetOrderByCustomerId(int customerId);
        public DtoOrder UpdateOrder(DtoOrder dtoOrder);
        public Task DeleteOrder(DtoOrder dtoOrder);
    }
}
