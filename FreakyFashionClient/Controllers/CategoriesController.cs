using FreakyFashionClient.IModels;
using FreakyFashionClient.IServices;
using FreakyFashionClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace FreakyFashionClient.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IManageEntityService<CategoryModel> manageEntityService;
        private readonly IDashboardViewModel<CategoryModel> dashboardViewModel;

        public CategoriesController(IManageEntityService<CategoryModel> manageEntityService,
            IDashboardViewModel<CategoryModel> dashboardViewModel)
        {
            this.manageEntityService = manageEntityService;
            this.dashboardViewModel = dashboardViewModel;
        }

        public IActionResult ListCategories()
        {
            return View();
        }
    }
}
