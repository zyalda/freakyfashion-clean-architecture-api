using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;

namespace InfrastructureLayer.Mapping
{
    public class AutoMapperDtoOrderItem: IMapperDtoOrderItem<DtoOrderItem, OrderItem>
    {
        public OrderItem MapDtoEntityToDistination(DtoOrderItem dtoOrderItem, OrderItem orderItem)
        {
            orderItem.OrderId = dtoOrderItem.OrderId;
            orderItem.ProductId = dtoOrderItem.ProductId;
            orderItem.Quantity = dtoOrderItem.Quantity;
            orderItem.UnitPrice = dtoOrderItem.UnitPrice;
            return orderItem;
        }

        public OrderItem MapEntityByParameters(int orderId, int customerId, int quantity, int unitPrice)
        {
            return new OrderItem
            {
                OrderId = orderId,
                ProductId = customerId,
                UnitPrice = quantity,
                Quantity = quantity,
            };
        }
    }
}
