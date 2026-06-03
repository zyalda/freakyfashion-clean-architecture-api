using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.Entites;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(FashionContext context) : base(context)
        {
        }
        public override IQueryable<Order> GetAll()
        {
            return _context.Orders.Include(x => x.OrderItems).AsNoTracking();
        }
    }
}
