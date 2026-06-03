using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using ApplicationLayer.IServices;
using DomainLayer.Entites;

namespace ApplicationLayer.Services
{
    public class OrderService : IOrderService
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapperUnitOfWork mapperUnitOfWork;

        public OrderService(IUnitOfWork unitOfWork, IMapperUnitOfWork mapperUnitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.mapperUnitOfWork = mapperUnitOfWork;
        }

        public async Task<DtoOrdersOrderItems> AddOrder(OrderRequest orderRequest)
        {
            if (orderRequest == null || orderRequest.Items == null || !orderRequest.Items.Any())
                return new DtoOrdersOrderItems { 
                    Order = new DtoOrder { StatusMessage = "Invalid order" }
                };

            var customer = unitOfWork.CustomerRepository.GetById(orderRequest.CustomerId);
            if(customer == null)
                return new DtoOrdersOrderItems
                {
                    Order = new DtoOrder { StatusMessage = "Invalid customer" }
                };

            var mappersOrder = mapperUnitOfWork.Mapper<Order, DtoOrder>();
            
            await unitOfWork.BeginTransactionAsync();
            
            try
            {
                var newOrder = new Order
                {
                    CustomerId = orderRequest.CustomerId,
                    TheTotal = 0
                };

                int computedTotal = 0;

                foreach (var cartItem in orderRequest.Items)
                {
                    var verifiedProduct = unitOfWork.ProductRepository.GetById(cartItem.ProductId);
                    if (verifiedProduct == null)
                    {
                        return new DtoOrdersOrderItems
                        {
                            Order = new DtoOrder() { StatusMessage = "Invalid Product Id" }
                        };
                        //throw new Exception($"Product verification failed for item ID: {cartItem.ProductId}");
                    }

                    int actualPrice = verifiedProduct.Price;
                    computedTotal += cartItem.Quantity * actualPrice;

                    var databaseItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = actualPrice,
                        Order = newOrder
                    };

                    newOrder.OrderItems.Add(databaseItem);
                }

                newOrder.TheTotal = computedTotal;

                unitOfWork.OrderRepository.Add(newOrder);
                unitOfWork.Complete();

                await unitOfWork.CommitTransactionAsync();

                DtoOrdersOrderItems newAddedOrder = GetOrderById(newOrder.Id).Result;

                var dtoNewOrder =  mappersOrder.MapEntity(newOrder);
                dtoNewOrder.StatusMessage = "The odrer has been added.";
                return newAddedOrder;
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public IEnumerable<DtoOrdersOrderItems> GetOrders()
        {
            var ordersIncludingItems = unitOfWork.OrderRepository.GetAll();

            var orders = new List<DtoOrdersOrderItems>();

            if (!ordersIncludingItems.Any())
                return orders;

            foreach (var order in ordersIncludingItems)
            {
                orders.Add( new DtoOrdersOrderItems
                 {
                    Order = mapperUnitOfWork.Mapper<Order, DtoOrder>().MapEntity(order),
                    OrderItems = order.OrderItems.Select(i => mapperUnitOfWork.Mapper<OrderItem, DtoOrderItem>().MapEntity(i)).ToList()
                });
            }

            return orders;
        }

        public async Task<IEnumerable<DtoOrdersOrderItems>> GetOrderByCustomerId(int customerId)
        {
            var ordersIncludingItems = unitOfWork.OrderRepository.GetAll().Where(x=>x.CustomerId == customerId);

            if (!ordersIncludingItems.Any())
                return null;

            var dtoOrdersIncludeItem = ordersIncludingItems.Select(x => new DtoOrdersOrderItems
            {
                Order = mapperUnitOfWork.Mapper<Order, DtoOrder>().MapEntity(x),
                OrderItems = x.OrderItems.Select(i => mapperUnitOfWork.Mapper<OrderItem, DtoOrderItem>().MapEntity(i)).ToList()
            }).ToList();
            
            return dtoOrdersIncludeItem;
        }

        public async Task<DtoOrdersOrderItems> GetOrderById(int id)
        {
            var orderById = unitOfWork.OrderRepository.GetAll().SingleOrDefault(x => x.Id == id);

            if (orderById == null)
                return new DtoOrdersOrderItems();

            var order = new DtoOrdersOrderItems
            {
                Order = mapperUnitOfWork.Mapper<Order, DtoOrder>().MapEntity(orderById),
                OrderItems = orderById.OrderItems.Select(i => mapperUnitOfWork.Mapper<OrderItem, DtoOrderItem>().MapEntity(i))
            };

            return order;
        }

        public async Task DeleteOrder(DtoOrder dtoOrder)
        {
            var orderById = unitOfWork.OrderRepository.GetAll().SingleOrDefault(x => x.Id == dtoOrder.Id);

            unitOfWork.OrderItemRepository.RemoveRange(orderById.OrderItems);
            unitOfWork.OrderRepository.Remove(orderById);
            unitOfWork.Complete();
        }

        public DtoOrder UpdateOrder(DtoOrder dtoOrder)
        {
            throw new NotImplementedException();
        }
    }
}