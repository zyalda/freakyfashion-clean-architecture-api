using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;

namespace InfrastructureLayer.Mapping
{
    public class AutoMapperOrderItem : IMapper<OrderItem, DtoOrderItem>
    {
        public DtoOrderItem MapEntity(OrderItem orderItem)
        {
            return new DtoOrderItem
            {
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                ProductId = orderItem.ProductId,
                OrderId = orderItem.OrderId,
            };
        }
    }
}
