using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;

namespace InfrastructureLayer.Mapping
{
    public class AutoMapperDtoCategory : IMapper<DtoCategory, Category>, IMapperDtoCategory<DtoCategory, Category>
    {
        public Category MapEntity(DtoCategory dtoCategory)
        {
            return new Category
            {
                Name = dtoCategory.Name,
                Id = dtoCategory.Id,
                Image = dtoCategory.Image,
                UrlSlug = dtoCategory.UrlSlug
            };
        }
        public Category MapEntityByParameters(string name, string image, string urlSlug)
        {
            return new Category
            {
                Name = name,
                Image = image,
                UrlSlug = urlSlug,
            };
        }

        public Category MapDtoEntityToDistination(DtoCategory dtoCategory, Category category)
        {
            category.Name = dtoCategory.Name;
            category.Id = dtoCategory.Id;
            category.Image = dtoCategory.Image;
            category.UrlSlug = dtoCategory.UrlSlug;

            return category;
        }
    }
}
