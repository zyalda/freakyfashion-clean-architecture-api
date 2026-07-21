using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using ApplicationLayer.IServices;
using ApplicationLayer.IStorageContainerServices;
using DomainLayer.Entites;
using InfrastructureLayer;
using InfrastructureLayer.Mapping;
using InfrastructureLayer.Repositories;
using InfrastructureLayer.StorageContainerServices;
using InfrastructureLayer.UnitOfWorks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RepositoriesDependencyInjectionProject
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositoriesInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Kopplar ihop sektionen i JSON med AzureBlobSettings klass!
            services.Configure<AzureBlobSettings>(configuration.GetSection(AzureBlobSettings.SectionName));

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IAzureBlobService, AzureBlobService>();
            services.AddScoped<IOrderNumberService, OrderNumberService>();
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

        public static IServiceCollection AddAuthenticationJwtBearer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["Jwt:Issuer"] ?? configuration["Jwt__Issuer"],
                    ValidAudience = configuration["Jwt:Audience"] ?? configuration["Jwt__Audience"],
                    ClockSkew = TimeSpan.FromSeconds(300),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        configuration["Jwt:Key"] ?? configuration["Jwt__Key"] ?? "mykeys1234567&%%485734579453%&//1255362"))// This hard ocdes string key is false.
                };
            });
            return services;
        }
    }
}
