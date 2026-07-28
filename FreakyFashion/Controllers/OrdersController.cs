using ApplicationLayer.Dto;
using ApplicationLayer.IServices;
using FreakyFashion.PaginationDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FreakyFashion.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : BaseController
    {
        private readonly ILogger<DtoProduct> logger;
        private readonly IOrderService orderService;
        public OrdersController(ILogger<DtoProduct> logger, IOrderService orderService)
        {
            this.logger = logger;
            this.orderService = orderService;
        }

        /// <summary>
        /// Submits a shopping cart checkout and saves the order to the database.
        /// </summary>
        /// <param name="request">The shopping cart containing the list of makeup items to purchase.</param>
        [HttpPost("Checkout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Checkout([FromBody] OrderRequest request)
        {            
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest(new { Message = "Welcome to your order. Please add items to Checkout." });
            }

            var customerToken = CustomerToken();
            int customerId = customerToken.Id;
            string existingCartJson = HttpContext.Session.GetString("CartToken")!;
            DtoOrdersOrderItems currentCart;

            if (!string.IsNullOrEmpty(existingCartJson))
            {
                currentCart = JsonSerializer.Deserialize<DtoOrdersOrderItems>(existingCartJson) ?? new DtoOrdersOrderItems();
            }
            else
            {
                currentCart = new DtoOrdersOrderItems();
            }

            if (currentCart.CustomerInfo != null && currentCart.CustomerInfo.Id != customerId && currentCart.Order != null && currentCart.Order.Id != 0)
            {
                logger.LogInformation($"Existing order found for customer  {currentCart.CustomerInfo.Id}. Appending new items.");

                //int orderId = currentCart.Order.Id;

                //var newOrderResult = await orderService.AddOrder(request, customerId, orderId);
                return BadRequest(new { Message = "Logged in customer miss match with order customer info." });
            }
            //else
            //{
            //    logger.LogInformation("No active order found. Creating a brand new checkout session.");
            //    //var newOrderResult = await orderService.AddOrder(request, customerId, 0);
            //    currentCart = newOrderResult;
            //}

            var newOrderResult = await orderService.AddOrderItemToCart(request, customerId);
            var combinedItems = currentCart.OrderItems.ToList();
            combinedItems.AddRange(newOrderResult.OrderItems);

            currentCart.CustomerInfo = newOrderResult.CustomerInfo;
            currentCart.OrderItems = combinedItems;
            currentCart.Order.TotalAmount = combinedItems.Sum(item => item.Quantity * item.UnitPrice);

            string updatedJsonTokenString = JsonSerializer.Serialize(currentCart, new JsonSerializerOptions { WriteIndented = true });
            HttpContext.Session.SetString("CartToken", updatedJsonTokenString);

            return StatusCode(StatusCodes.Status201Created, currentCart);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllOrders")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<DtoOrdersOrderItems>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResponse<DtoOrdersOrderItems>>> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                int currentPage = page < 1 ? 1 : page;
                int currentSize = pageSize < 1 ? 12 : pageSize;
                var orders = orderService.GetOrders();

                if (orders == null)
                    return Ok(new List<DtoOrdersOrderItems>());

                var orderList = orders
                    .Skip((currentPage - 1) * currentSize)
                    .Take(currentSize)
                    .ToList();

                var ordersPagintionModel = new PagedResponse<DtoOrdersOrderItems>
                {
                    EntitiesDto = orderList,
                    CurrentPage = currentPage,
                    PageSize = currentSize,
                    TotalRecords = orders.Count()
                };
                return Ok(ordersPagintionModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error caught inside {nameof(GetAllOrders)} endpoint flow.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetOrderById/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoOrdersOrderItems))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DtoOrdersOrderItems>> GetOrderById([FromRoute] int id)
        {
            try
            {
                var response = await orderService.GetOrderById(id);
                var order = response.Order;

                if (order == null)
                {
                    logger.LogError($"The order with id {id} not found.");
                    return Ok($"404 The order with id {id} not found.");
                }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(orderService)} in {nameof(DeleteOrder)}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrderByCustomer")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<DtoOrdersOrderItems>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<DtoOrdersOrderItems>>> GetOrderByCustomer([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var customerToken = CustomerToken();

                if (customerToken == null)
                    return Ok("You are un authorized.");
                int customerId = customerToken.Id;

                var orders = await orderService.GetOrderByCustomerId(customerId);

                if (orders == null)
                {
                    logger.LogError($"The order with customer id {customerId} not found.");
                    return Ok($"404 The order with customer id {customerId} not found.");
                }

                int currentPage = page < 1 ? 1 : page;
                int currentSize = pageSize < 1 ? 12 : pageSize;


                var orderList = orders
                    .Skip((currentPage - 1) * currentSize)
                    .Take(currentSize).ToList();

                var ordersPagintionModel = new PagedResponse<DtoOrderListItem>
                {
                    CustomerInfo = customerToken,
                    EntitiesDto = orderList,
                    CurrentPage = currentPage,
                    PageSize = currentSize,
                    TotalRecords = orders.Count()
                };

                return Ok(ordersPagintionModel);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(orderService)} in {nameof(GetOrderByCustomer)}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteOrder/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteOrder([FromRoute] int id)
        {
            try
            {
                var response = await orderService.GetOrderById(id);
                var order = response.Order;

                if (order == null)
                {
                    logger.LogError($"The order with id {id} not found.");
                    return Ok($"404 The order with id {id} not found.");
                }

                await orderService.DeleteOrder(order);
                return Ok($"The order {order.Id} is deleted.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(orderService)} in {nameof(DeleteOrder)}");
                return BadRequest(ex.Message);
            }
        }
    }
}

/*

    // 2. DIN SMARTA KONTROLL: Har vi redan kundinfo och en aktiv order? 🔎
    if (currentCart.CustomerInfo != null && currentCart.Order != null && currentCart.Order.Id != 0)
    {
        _logger.LogInformation($"Existing order {currentCart.Order.Id} found for customer. Appending new items.");

        // Gör om IEnumerable till en lista så vi kan lägga till de nya varorna
        var existingItemsList = currentCart.OrderItems.ToList();

        // Mappa och lägg till de nya artiklarna från requesten till den befintliga listan
        foreach (var item in request.Items)
        {
            existingItemsList.Add(new DtoOrderItem 
            { 
                ProductId = item.ProductId, 
                Quantity = item.Quantity,
                Price = item.Price
            });
        }

        // Sätt tillbaka den uppdaterade listan till din IEnumerable
        currentCart.OrderItems = existingItemsList;
    }
    else
    {
        _logger.LogInformation("No active order found. Creating a brand new checkout session.");

        // Skapa en helt ny order via din service (som sätter Kundinfo, Order och de första varorna)
        var newOrderResult = await orderService.AddOrder(request);

        currentCart.OrderNumber = newOrderResult.OrderNumber;
        currentCart.CustomerInfo = newOrderResult.CustomerInfo;
        currentCart.Order = newOrderResult.Order;
        currentCart.OrderItems = newOrderResult.OrderItems;
    }

    // 3. SPARA OM: Serialisera den enda, rena DTO-strukturen tillbaka till sessionen
    string updatedJson = JsonSerializer.Serialize(currentCart, new JsonSerializerOptions { WriteIndented = true });
    HttpContext.Session.SetString("CartToken", updatedJson);

    return StatusCode(StatusCodes.Status201Created, currentCart);
}
 */
