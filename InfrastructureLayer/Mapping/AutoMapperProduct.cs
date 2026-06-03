using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;

namespace InfrastructureLayer.Mapping
{
    public class AutoMapperProduct : IMapper<Product, DtoProduct>
    {
        public DtoProduct MapEntity(Product product)
        {
            return new DtoProduct
            {
                Name = product.Name,
                Id = product.Id,
                Description = product.Description,
                Image = product.Image,
                Price = product.Price,
                UrlSlug = product.UrlSlug
            };
        }
    }
}
