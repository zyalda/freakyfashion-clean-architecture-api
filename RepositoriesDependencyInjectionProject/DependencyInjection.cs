using System.Text;
using DomainLayer.Entites;
using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using InfrastructureLayer.Mapping;
using Microsoft.IdentityModel.Tokens;
using InfrastructureLayer.UnitOfWorks;
using InfrastructureLayer.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ApplicationLayer.IStorageContainerServices;
using InfrastructureLayer.StorageContainerServices;

namespace RepositoriesDependencyInjectionProject
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositoriesInjection(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IAzureBlobService, AzureBlobService>();
            services.AddScoped<IMapper<Product, DtoProduct>, AutoMapperProduct>();
            services.AddScoped<IMapper<Category, DtoCategory>, AutoMapperCategory>();
            services.AddScoped<IMapper<Customer, DtoCustomer>, AutoMapperCustomer>();
            services.AddScoped<IMapper<Order, DtoOrder>, AutoMapperOrder>();
            services.AddScoped<IMapper<OrderItem, DtoOrderItem>, AutoMapperOrderItem>();
            services.AddScoped<IMapperUnitOfWork, MapperUnitOfWork>();
            services.AddScoped<IMapperDtoProduct<DtoProduct, Product>, AutoMapperDtoProduct>();
            services.AddScoped<IMapperDtoCategory<DtoCategory, Category>, AutoMapperDtoCategory>();
            services.AddScoped<IMapperDtoOrder<DtoOrder, Order>, AutoMapperDtoOrder>();
            services.AddScoped<IMapperDtoOrderItem<DtoOrderItem, OrderItem>, AutoMapperDtoOrderItem>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddAuthenticationJwtBearer(this IServiceCollection services)
        {
            //Add JWT Code here.
            services.AddAuthentication(opt =>
            {
                //opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               //Här säger vi hur vi skall jobba med JWT
               .AddJwtBearer(opt =>
               {
                   opt.TokenValidationParameters = new TokenValidationParameters
                   {
                       //Issuer är vem (vilken server) som utfärdat en JWT token
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = "http://localhost:3000",
                       ValidAudience = "http://localhost:3000",
                       ClockSkew = TimeSpan.FromSeconds(300),
                       IssuerSigningKey =
                  new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mykey1234567&%%485734579453%&//1255362"))
                   };
               });
            return services;
        }

    }
}
