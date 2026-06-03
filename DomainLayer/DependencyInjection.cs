using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DomainLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFashionDataBaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<FashionContext>(options =>
               options.UseSqlServer(defaultConnectionString, x => x.MigrationsAssembly("DomainLayer")));

            return services;
        }
    }
}
