using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;

namespace InfrastructureLayer.Mapping
{
    public class AutoMapperOrder : IMapper<Order, DtoOrder>
    {
        public DtoOrder MapEntity(Order order)
        {
            return new DtoOrder
            {
                Id = order.Id,
                TotalAmount = order.TheTotal
            };
        }
    }
}
