using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.Entites;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(FashionContext context) : base(context)
        {
        }

        public override IQueryable<Category> GetAll()
        {
            var categories = _context.Categories.Include(x => x.Products).AsNoTracking();
            return categories;
        }

        public override Category GetById(int id)
        {
            var category = _context.Categories.Include(x => x.Products).Where(x => x.Id == id).FirstOrDefault();
            return category;
        }

        public Category GetBySlug(string slug)
        {
            var category = _context.Categories.Include(x => x.Products).Where(x => x.UrlSlug.ToLower() == slug.ToLower()).FirstOrDefault();
            return category;
        }
    }
}
