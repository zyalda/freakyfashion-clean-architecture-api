using ApplicationLayer.IServices;
using ApplicationLayer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationCore(this IServiceCollection services)
        {
            //Row below is in case we add AutoMapper objects to DB entites.
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICloudAssetResolver, CloudAssetResolver>();
            services.AddScoped<IGenerateUrlSlugClass, GenerateUrlSlugClass>();
            services.AddScoped<IAuthenticateUserService, AuthenticateUserService>();

            return services;
        }
    }
}
