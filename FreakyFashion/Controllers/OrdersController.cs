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
    public class OrdersController : ControllerBase
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
                return BadRequest(new { Message = "Your shopping cart is empty." });
            }

            var newOrder = await orderService.AddOrder(request);

            var autherizeSessionData = new DtoOrdersOrderItems
            {
                Order = newOrder.Order,
                OrderItems = newOrder.OrderItems
            };

            // Serialisera och spara i sessionen
            string jsonTokenString = JsonSerializer.Serialize(autherizeSessionData);

            HttpContext.Session.SetString("CartToken", jsonTokenString);

            return StatusCode(StatusCodes.Status201Created, newOrder);
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
        /// <param name="customerid"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrderByCustomerId/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<DtoOrdersOrderItems>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<DtoOrdersOrderItems>>> GetOrderByCustomerId([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var orders = await orderService.GetOrderByCustomerId(id);

                if (orders == null)
                {
                    logger.LogError($"The order with customer id {id} not found.");
                    return Ok($"404 The order with customer id {id} not found.");
                }

                int currentPage = page < 1 ? 1 : page;
                int currentSize = pageSize < 1 ? 12 : pageSize;

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
                logger.LogError($"Error in {nameof(orderService)} in {nameof(GetOrderByCustomerId)}");
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
