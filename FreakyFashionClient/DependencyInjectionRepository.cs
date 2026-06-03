using FreakyFashionClient.IModels;
using FreakyFashionClient.IServices;
using FreakyFashionClient.Models;
using FreakyFashionClient.Services;

namespace FreakyFashionClient
{
    public static class DependencyInjectionRepository
    {
        public static IServiceCollection AddServicesInjection(this IServiceCollection services)
        {
            services.AddScoped<ILoginResult, LoginResult>();
            services.AddScoped<IDashboardViewModel<ProductModel>, DashboardViewModel>();
            services.AddScoped<IDashboardViewModel<CategoryModel>, DashboardCategoriesViewModel>();
            services.AddScoped<IManageEntityService<ProductModel>, ManageProductService>();
            services.AddScoped<IGetAccessDataAndTokenService, GetAccessDataAndTokenService>();
            return services;
        }
    }
}
