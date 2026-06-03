using DomainLayer.Entites;

namespace ApplicationLayer.Interfaces
{
    public interface ICategoryRepository: IGenericRepository<Category>
    {
        public Category GetById(int id);

        public Category GetBySlug(string slug);
    }
}
