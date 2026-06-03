using FreakyFashionClient.IServices;
using FreakyFashionClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace FreakyFashionClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGetAccessDataAndTokenService getAccessDataAndTokenService;
        public HomeController(IGetAccessDataAndTokenService getAccessDataAndTokenService)
        {
            this.getAccessDataAndTokenService = getAccessDataAndTokenService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["winterImage"] = "wintermakeup.jpg";
            ViewData["autumnImage"] = "autumnmakeup.jpg";
            return View(new UserSubmissionModel());
        }

        [AllowAnonymous]
        public IActionResult SubmitData(UserSubmissionModel model)
        {
            ViewData["winterImage"] = "wintermakeup.jpg";
            ViewData["autumnImage"] = "autumnmakeup.jpg";
            // Verify that the received data matches your rules
            if (ModelState.IsValid)
            {
                var response = getAccessDataAndTokenService.GetTokenAccesstAsync(model.UserName, model.PassWord);
                var data = response.Result.UserManagerResponseModelData;

                // Exempel: Ett objekt med användardata och roller
                var autherizeSessionData = new
                {
                    AccessToken = data.AccessToken,
                    TokenType = data.TokenType,
                    ExpiresIn = data.ExpiresIn
                };

                // Serialisera och spara i sessionen
                string jsonTokenString = JsonSerializer.Serialize(autherizeSessionData);

                HttpContext.Session.SetString("Token", jsonTokenString);

                TempData["ProductFilterData"] = JsonSerializer.Serialize(data);

                return RedirectToAction("AddProduct", "Products");
            }
            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
