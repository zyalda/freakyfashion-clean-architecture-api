using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;

namespace InfrastructureLayer.Mapping
{
    public class AutoMapperDtoOrder: IMapperDtoOrder<DtoOrder, Order>
    {
        public Order MapDtoEntityToDistination(DtoOrder dtoOrder, Order order)
        {
            order.TheTotal = dtoOrder.TotalAmount;
            order.Id = dtoOrder.Id;
            return order;
        }

        public Order MapEntityByParameters(int total, int customerId)
        {
            return new Order
            {
                TheTotal = total,
                CustomerId = customerId,
            };
        }
    }
}
