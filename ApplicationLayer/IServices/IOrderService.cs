using ApplicationLayer.Dto;

namespace ApplicationLayer.IServices
{
    public interface IOrderService
    {
        public Task<DtoOrdersOrderItems> AddOrder(OrderRequest orderRequest);
        public IEnumerable<DtoOrdersOrderItems> GetOrders();
        public Task<DtoOrdersOrderItems> GetOrderById(int id);
        public Task<IEnumerable<DtoOrdersOrderItems>> GetOrderByCustomerId(int customerId);
        public DtoOrder UpdateOrder(DtoOrder dtoOrder);
        public Task DeleteOrder(DtoOrder dtoOrder);
    }
}
