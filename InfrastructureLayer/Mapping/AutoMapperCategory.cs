using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;

namespace InfrastructureLayer.Mapping
{
    public class AutoMapperCategory : IMapper<Category, DtoCategory>
    {
        public DtoCategory MapEntity(Category category)
        {
            return new DtoCategory
            {
                Name = category.Name,
                Id = category.Id,
                Image = category.Image,
                UrlSlug = category.UrlSlug
            };
        }
    }
}
