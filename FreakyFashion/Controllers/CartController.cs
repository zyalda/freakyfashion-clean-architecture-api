using ApplicationLayer.Dto;
using ApplicationLayer.IServices;
using ApplicationLayer.IStorageContainerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.Text.Json;

namespace FreakyFashion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ILogger<DtoProduct> logger;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IOrderNumberService _orderNumberService;
        public CartController(ILogger<DtoProduct> logger, IAzureBlobService azureBlobService, IOrderNumberService orderNumberService)
        {
            this.logger = logger;
            this._azureBlobService = azureBlobService;
            this._orderNumberService = orderNumberService;
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

                if (cartOrders == null)
                {
                    logger.LogError($"{customerToken.Name} you have not ordered yet.");
                    return Ok($"{customerToken.Name} you have not ordered yet.");
                }

                cartOrders.CustomerInfo.Id = customerId;
                int orderId = cartOrders.Order.Id;
                cartOrders.CustomerInfo.Name = customerToken.Name;
                string generatedOrderNumber =  _orderNumberService.Generate(orderId);
                cartOrders.OrderNumber = generatedOrderNumber;

                string jsonContent = JsonSerializer.Serialize(cartOrders);

                string fileName = await _azureBlobService.UploadBlobAsync(jsonContent, generatedOrderNumber);

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    logger.LogError($"Failed to generate order number for order ID {cartOrders.Order.Id}. Blob upload might have failed.");
                    return BadRequest("Could not process your order. Please try again or contact support.");
                }

                return Ok(cartOrders);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(OrderCart)}");
                return BadRequest(ex.Message);
            }
        }
        public DtoCustomer CustomerToken()
        {
            var jsonString = HttpContext.Session.GetString("CustomerToken");
            if (!string.IsNullOrEmpty(jsonString))
            {
                // Deserialise.
                var customerSessionData = JsonSerializer.Deserialize<DtoCustomer>(jsonString);
                if (customerSessionData != null)

                    return customerSessionData;
            }
            return null;
        }

        public DtoOrdersOrderItems CartToken()
        {
            var jsonString = HttpContext.Session.GetString("CartToken");
            if (!string.IsNullOrEmpty(jsonString))
            {
                // Deserialise.
                var orderSessionData = JsonSerializer.Deserialize<DtoOrdersOrderItems>(jsonString);
                if (orderSessionData != null)

                    return orderSessionData;
            }
            return null;
        }
    }
}
