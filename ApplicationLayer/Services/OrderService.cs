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

        public async Task<DtoOrdersOrderItems> AddOrderItemToCart(OrderRequest orderRequest, int customerId)
        {
            if (orderRequest == null || orderRequest.Items == null || !orderRequest.Items.Any())
                return new DtoOrdersOrderItems
                {
                    Order = new DtoOrder { StatusMessage = "Invalid order" }
                };

            var customer = unitOfWork.CustomerRepository.GetById(customerId);
            if (customer == null)
                return new DtoOrdersOrderItems
                {
                    Order = new DtoOrder { StatusMessage = "Invalid customer" }
                };

            try
            {
                int computedTotal = 0;
                var dtoList = new List<DtoOrderItem>();

                foreach (var cartItem in orderRequest.Items)
                {
                    var verifiedProduct = unitOfWork.ProductRepository.GetById(cartItem.ProductId);
                    if (verifiedProduct == null)
                    {
                        return new DtoOrdersOrderItems
                        {
                            Order = new DtoOrder() { StatusMessage = "Invalid Product Id" }
                        };
                    }

                    int actualPrice = verifiedProduct.Price;
                    computedTotal += cartItem.Quantity * actualPrice;

                    var dtoItem = new DtoOrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = actualPrice
                    };

                    dtoList.Add(dtoItem);
                }

                var newAddedOrder = new DtoOrdersOrderItems();

                newAddedOrder.CustomerInfo.Name = customer.Name;
                newAddedOrder.CustomerInfo.Id = customer.Id;
                newAddedOrder.Order.Id = 0; //new order pending, not created in DB yet.
                newAddedOrder.Order.TotalAmount = computedTotal;
                newAddedOrder.OrderItems = dtoList;

                if (newAddedOrder.Order != null)
                {
                    newAddedOrder.Order.StatusMessage = "The order has been updated/added successfully.";
                }

                return newAddedOrder;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DtoOrdersOrderItems> AddOrder(OrderRequest orderRequest, int customerId) //, int orderID)
        {
            if (orderRequest == null || orderRequest.Items == null || !orderRequest.Items.Any())
                return new DtoOrdersOrderItems { 
                    Order = new DtoOrder { StatusMessage = "Invalid order" }
                };

            var customer = unitOfWork.CustomerRepository.GetById(customerId);
            if(customer == null)
                return new DtoOrdersOrderItems
                {
                    Order = new DtoOrder { StatusMessage = "Invalid customer" }
                };

            var mappersOrder = mapperUnitOfWork.Mapper<Order, DtoOrder>();
            
            await unitOfWork.BeginTransactionAsync();
            
            try
            {
                int computedTotal = 0;

                   var order = new Order
                    {
                        CustomerId = customerId,
                        TheTotal = 0
                    };   
                
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
                        Order = order
                    };

                    order.OrderItems.Add(databaseItem);
                }

                order.TheTotal = computedTotal;

                unitOfWork.OrderRepository.Add(order);

                unitOfWork.Complete();

                await unitOfWork.CommitTransactionAsync();

                DtoOrdersOrderItems newAddedOrder = await GetOrderById(order.Id);

                if (newAddedOrder.CustomerInfo == null)
                {
                    newAddedOrder.CustomerInfo = new DtoCustomer();
                }

                newAddedOrder.CustomerInfo.Name = customer.Name;
                newAddedOrder.CustomerInfo.Id = customer.Id;

                var dtoNewOrder =  mappersOrder.MapEntity(order);
                if (newAddedOrder.Order != null)
                {
                    newAddedOrder.Order.StatusMessage = "The order has been added successfully.";
                }

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

        public async Task<IEnumerable<DtoOrderListItem>> GetOrderByCustomerId(int customerId)
        {
            var ordersIncludingItems = unitOfWork.OrderRepository.GetAll().Where(x=>x.CustomerId == customerId);

            if (!ordersIncludingItems.Any())
                return null;

            var dtoOrdersIncludeItem = ordersIncludingItems.Select(x => new DtoOrderListItem
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