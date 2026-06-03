using ApplicationLayer.Dto;

namespace ApplicationLayer.IServices
{
    public interface ICategoryService
    {        
        public IEnumerable<CategoryWithProductsDto> GetCategories();
        public CategoryWithProductsDto GetCategoryById(int id);
        public CategoryWithProductsDto GetCategoryBySlug(string urlSlug);
        public DtoCategory AddCategory(CategoryUploadForm dtoCategory);
        public DtoCategory UpdateCategory(DtoCategory dtoProduct);
        public void DeleteCategory(DtoCategory dtoProduct);
    }
}