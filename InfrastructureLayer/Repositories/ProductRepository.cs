using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.Entites;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repositories
{
    public class ProductRepository :GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(FashionContext context) : base(context)
        {
        }

        public override IQueryable<Product> GetAll()
        {
           return _context.Products.Include(x => x.Category).AsNoTracking();
        }
    }
}
