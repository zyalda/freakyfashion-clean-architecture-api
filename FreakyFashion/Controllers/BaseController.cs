using ApplicationLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FreakyFashion.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected DtoCustomer? CustomerToken()
        {
            var jsonString = HttpContext.Session.GetString("CustomerToken");
            if (!string.IsNullOrEmpty(jsonString))
            {
                return JsonSerializer.Deserialize<DtoCustomer>(jsonString);
            }
            return null;
        }
    }
}
