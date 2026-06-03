using FreakyFashionClient.Models;

namespace FreakyFashionClient.IServices
{
    public interface IManageEntityService<T> where T : class
    {
        public Task<IEnumerable<T>> ListEnitity();

        public Task<T> AddEntity(string token, string name, string description, int price, string image, string urlSlug, string category);

        public Task<T> UpdateEntity(int id, string token, ProductModel model);

        public Task<bool> DeleteEntity(int id, string token);
        public Task<T> GetProductById(int id);
        public Task<IEnumerable<T>> GetProductByUrlSlug(string urlSlug);
    }
}