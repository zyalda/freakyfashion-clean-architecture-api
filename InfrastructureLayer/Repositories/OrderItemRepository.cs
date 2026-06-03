using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.Entites;

namespace InfrastructureLayer.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(FashionContext context) : base(context)
        {
        }
    }
}
