using Azure;
using FreakyFashionClient.IModels;
using FreakyFashionClient.IServices;
using FreakyFashionClient.Models;
using FreakyFashionClient.PaginationDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace FreakyFashionClient.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IManageEntityService<ProductModel> manageEntityService;
        private readonly IDashboardViewModel<ProductModel> dashboardViewModel;
        public ProductsController(IManageEntityService<ProductModel> manageEntityService,
            IDashboardViewModel<ProductModel> dashboardViewModel)
        {
            this.manageEntityService = manageEntityService;
            this.dashboardViewModel = dashboardViewModel;
        }

        private void RenderImagesViews()
        {
            ViewData["winterImage"] = "summermakeup.jpg";
            ViewData["autumnImage"] = "springmakeup.jpg";
        }

        public UserManagerResponseModel Token()
        {
            var jsonString = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(jsonString))
            {
                // Deserialise.
                var userSessionData = JsonSerializer.Deserialize<UserManagerResponseModel>(jsonString);

                var token = userSessionData.AccessToken;
                var tokenType = userSessionData.TokenType;
                var expire = userSessionData.ExpiresIn;
                
                return userSessionData;
            }
            return null;
        }

        [HttpGet("Products")]
        public IActionResult ListProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            ViewData["title"] = "Products!";

            if (Token() == null)
                ViewData["header"] = "You need to login to update or delete a product.";
            else
                dashboardViewModel.UserManagerResponseModel = Token();

            RenderImagesViews();

            int currentPage = page < 1 ? 1 : page;
            int currentSize = pageSize < 1 ? 12 : pageSize;


            var response = manageEntityService.ListEnitity();

            // Fetch the segment of products for the active page
            var ProductList = response.Result.ToList()
                .Skip((currentPage - 1) * currentSize)
                .Take(currentSize)
                .ToList();

            var ProductsPagintionModel = new PagedResponse
            {
                Products = ProductList,
                CurrentPage = currentPage,
                PageSize = currentSize,
                TotalRecords = response.Result.Count()
            };

            dashboardViewModel.PagedResponse = ProductsPagintionModel;

            //dashboardViewModel.ProductModelList = response.Result.ToList();

            // Check if TempData contains your key
            if (TempData["ProductFilterData"] is string filterJson)
            {
                var redirectData = JsonSerializer.Deserialize<string>(filterJson);
            }
            return View(dashboardViewModel);
        }

        public IActionResult GetProductById(DashboardViewModel model)
        {
            ViewData["title"] = "GetProductById!";
           
            RenderImagesViews();
            // Check if TempData contains your key
            if (TempData["GetProductByIdData"] is string filterJson)
            {
                var redirectData = JsonSerializer.Deserialize<ProductModel>(filterJson);
                dashboardViewModel.EntityModel = redirectData;
                ViewData["NotFound"] = dashboardViewModel.EntityModel.StatusMessage;

                return View(dashboardViewModel);
            }
            else
            {
                dashboardViewModel.UserManagerResponseModel = new UserManagerResponseModel();
                return View(dashboardViewModel);
            }
        }
        public IActionResult SubmitGetProductById(DashboardViewModel model)
        {
            var response = manageEntityService.GetProductById(model.EntityModel.Id);
            if (response.Result != null)
            {
                TempData["GetProductByIdData"] = JsonSerializer.Serialize(response.Result);
                return RedirectToAction("GetProductById");
            }
                return RedirectToAction("GetProductById");          
        }

        public IActionResult GetProductByUrlSlug()
        {
            ViewData["title"] = "GetProductByUrlSlug!";

            RenderImagesViews();
            // Check if TempData contains your key
            if (TempData["GetProductByUrlSlugData"] is string filterJson)
            {
                List<ProductModel>? products = JsonSerializer.Deserialize<List<ProductModel>>(filterJson);
                dashboardViewModel.EntityModelList = products;

                return View(dashboardViewModel);
            }
            else
            {
                dashboardViewModel.UserManagerResponseModel = new UserManagerResponseModel();
                return View(dashboardViewModel);
            }
        }
        public IActionResult SubmitGetProductByUrlSlug(DashboardViewModel model)
        {
            var response = manageEntityService.GetProductByUrlSlug(model.EntityModel.UrlSlug);
            if (response.Result != null)
            {
                var urlList = response.Result.ToList();
                TempData["GetProductByUrlSlugData"] = JsonSerializer.Serialize(response.Result);
                return RedirectToAction("GetProductByUrlSlug");
            }
            return View("GetProductByUrlSlug");
        }
        public IActionResult AddProduct()
        {
            ViewData["title"] = "Add a new product.";

            if (Token() == null)
                ViewData["header"] = "You need to login to add a new product.";
            else
                dashboardViewModel.UserManagerResponseModel = Token();

            RenderImagesViews();

            if (TempData["ProductFilterData"] is string filterJson)
            {
                var redirectData = JsonSerializer.Deserialize<UserManagerResponseModel>(filterJson);
                dashboardViewModel.UserManagerResponseModel = redirectData;

                return View(dashboardViewModel);
            }

            return View(dashboardViewModel);
        }

        [HttpPost]
        public IActionResult SubmitProduct(DashboardViewModel model)
        {
            RenderImagesViews();

            if (model.UserManagerResponseModel.AccessToken != null)
            {
                var response = manageEntityService.AddEntity(model.UserManagerResponseModel.AccessToken, model.EntityModel.Name, model.EntityModel.Description, model.EntityModel.Price, model.EntityModel.ImageFile.FileName, model.EntityModel.UrlSlug, model.EntityModel.Category);

                dashboardViewModel.EntityModelList.Add(response.Result);

                return RedirectToAction("ListProducts");
            }
            return View("AddProduct", dashboardViewModel);
        }

        [HttpPost]
        public IActionResult Update(DashboardViewModel model)
        {
            RenderImagesViews();
            if (model.UserManagerResponseModel.AccessToken != null)
            {
                TempData["ProductUpdateData"] = JsonSerializer.Serialize(model);
                return RedirectToAction("UpdateProduct");
            }
            return RedirectToAction("ListProducts");
        }

        public IActionResult UpdateProduct()
        {
            ViewData["title"] = "Update product";
            RenderImagesViews();

            if (TempData["ProductUpdateData"] is string filterJson)
            {
                var redirectData = JsonSerializer.Deserialize<DashboardViewModel>(filterJson);
                dashboardViewModel.UserManagerResponseModel.AccessToken = redirectData.UserManagerResponseModel.AccessToken;
                dashboardViewModel.EntityModel = redirectData.EntityModel;

                return View(dashboardViewModel);
            }
            return View();
        }

        [HttpPost]
        public IActionResult SubmitUpdateProduct(DashboardViewModel model)
        {
            RenderImagesViews();

            // Verify that the received data matches your rules
            if (model.UserManagerResponseModel.AccessToken != null)
            {
                var response = manageEntityService.UpdateEntity(model.EntityModel.Id, model.UserManagerResponseModel.AccessToken, model.EntityModel);

                if (response.Result != null)
                    return RedirectToAction("ListProducts");

                return RedirectToAction("ListProducts", model.UserManagerResponseModel.AccessToken);
            }
            return View("AddProduct", dashboardViewModel);
        }

        [HttpPost]
        public IActionResult SubmitDeleteProduct(DashboardViewModel model)
        {
            if (model.UserManagerResponseModel.AccessToken != null)
            {
                var response = manageEntityService.DeleteEntity(model.EntityModel.Id, model.UserManagerResponseModel.AccessToken);

                if (response.Result == true)
                    return RedirectToAction("ListProducts");
            }
            return RedirectToAction("ListProducts");
        }
    }
}
