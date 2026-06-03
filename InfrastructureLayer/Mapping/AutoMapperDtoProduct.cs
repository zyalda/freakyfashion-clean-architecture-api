using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;

namespace InfrastructureLayer.Mapping
{
    public class AutoMapperDtoProduct : IMapperDtoProduct<DtoProduct, Product>
    {
        public Product MapDtoEntityToDistination(DtoProduct dtoProduct, Product product)
        {
            product.Name = dtoProduct.Name;
            product.Id = dtoProduct.Id;
            product.Description = dtoProduct.Description;
            product.Image = dtoProduct.Image;
            product.Price = dtoProduct.Price;
            product.UrlSlug = dtoProduct.UrlSlug;

            return product;
        }

        public Product MapEntityByParameters(string name, string description, string image, string urlSlug, int price)
        {
            return new Product
            {
                Name = name,
                Description = description,
                Image = image,
                Price = price,
                UrlSlug = urlSlug
            };
        }
    }
}
