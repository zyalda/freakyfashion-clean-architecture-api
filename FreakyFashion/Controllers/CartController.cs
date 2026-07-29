using ApplicationLayer.Dto;
using ApplicationLayer.IServices;
using ApplicationLayer.IStorageContainerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FreakyFashion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : BaseController
    {
        private readonly ILogger<DtoProduct> logger;
        private readonly IOrderService orderService;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IOrderNumberService _orderNumberService;
        public CartController(ILogger<DtoProduct> logger, IAzureBlobService azureBlobService, IOrderNumberService orderNumberService, IOrderService orderService)
        {
            this.logger = logger;
            this._azureBlobService = azureBlobService;
            this._orderNumberService = orderNumberService;
            this.orderService = orderService;
        }

        [HttpGet]
        [Route("OrderCart")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoOrdersOrderItems))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DtoOrdersOrderItems>> OrderCart()
        {
            try
            {
                var customerToken = CustomerToken();

                if (customerToken == null)
                    return Ok("You are un authorized.");
                int customerId = customerToken.Id;

                var cartOrders = CartToken();

                if (cartOrders == null || cartOrders.OrderItems == null || !cartOrders.OrderItems.Any())
                {
                    logger.LogError($"{customerToken.Name} you have not ordered yet.");
                    return Ok($"{customerToken.Name} you have not ordered yet.");
                }
                
                cartOrders.CustomerInfo.Name = customerToken.Name;
                cartOrders.CustomerInfo.Id = customerToken.Id;

                return Ok(cartOrders);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(OrderCart)}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ConfirmTheOrder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoOrdersOrderItems))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DtoOrdersOrderItems>> ConfirmTheOrder()
        {
            try
            {
                var customerToken = CustomerToken();

                if (customerToken == null)
                    return Ok("You are un authorized.");
                int customerId = customerToken.Id;

                var cartOrders = CartToken();

                if (cartOrders.Order.Id > 0)
                    return BadRequest($"This order with {cartOrders.Order.Id} id is already confirmed");

                if (cartOrders == null || cartOrders.OrderItems == null || !cartOrders.OrderItems.Any())
                {
                    logger.LogError($"{customerToken.Name} you have no order to confirm.");
                    return Ok($"Cannot confirm order. {customerToken.Name} you have no order to confirm.");
                }

                var orderRequest = new OrderRequest
                {
                    Items = cartOrders.OrderItems.Select(item => new ShoppingList
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList()
                };

                var newOrderResult = await orderService.AddOrder(orderRequest, customerId);

                string updatedJsonTokenString = JsonSerializer.Serialize(newOrderResult, new JsonSerializerOptions { WriteIndented = true });
                HttpContext.Session.SetString("CartToken", updatedJsonTokenString);
                newOrderResult.CustomerInfo.Name = customerToken.Name;
                newOrderResult.CustomerInfo.Id = customerToken.Id;
                string generatedOrderNumber =  _orderNumberService.Generate(newOrderResult.Order.Id);
                newOrderResult.OrderNumber = generatedOrderNumber;

                string jsonContent = JsonSerializer.Serialize(newOrderResult);

                string fileName = await _azureBlobService.UploadBlobAsync(jsonContent, generatedOrderNumber);

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    logger.LogError($"Failed to generate order number for order ID {newOrderResult.Order.Id}. Blob upload might have failed.");
                    return BadRequest("Could not process your order. Please try again or contact support.");
                }

                return Ok(newOrderResult);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(ConfirmTheOrder)}");
                return BadRequest(ex.Message);
            }
        }

        public DtoOrdersOrderItems CartToken()
        {
            var jsonString = HttpContext.Session.GetString("CartToken");
            
            if (string.IsNullOrEmpty(jsonString))
            {
                return new DtoOrdersOrderItems();
            }

            var orderList = JsonSerializer.Deserialize<DtoOrdersOrderItems>(jsonString);

            return orderList;
        }
    }
}
