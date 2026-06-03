using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.Entites;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(FashionContext context) : base(context)
        {
        }
    }
}
