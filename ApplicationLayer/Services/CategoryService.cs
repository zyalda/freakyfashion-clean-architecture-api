using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using ApplicationLayer.IServices;
using DomainLayer.Entites;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenerateUrlSlugClass generateUrlSlugClass;
        private readonly IMapperUnitOfWork mapperUnitOfWork;

        public CategoryService(IUnitOfWork unitOfWork, IMapperUnitOfWork mapperUnitOfWork, IMapperDtoCategory<DtoCategory, Category> mapperDtoCategory , IGenerateUrlSlugClass generateUrlSlugClass)
        {
            _unitOfWork = unitOfWork;
            this.mapperUnitOfWork = mapperUnitOfWork;
            this.generateUrlSlugClass = generateUrlSlugClass;
        }

        public IEnumerable<CategoryWithProductsDto> GetCategories()
        {
            var categoriesIncludProductsFromDb = _unitOfWork.CategoryRepository.GetAll().Include(x => x.Products).AsNoTracking().ToList();

            var categoriesIncludProducts = categoriesIncludProductsFromDb.Select(x => new CategoryWithProductsDto
            {
                Category = mapperUnitOfWork.Mapper<Category, DtoCategory>().MapEntity(x),
                Products = x.Products.Select(i => mapperUnitOfWork.Mapper<Product, DtoProduct>().MapEntity(i)).ToList()
            }).ToList();

            return categoriesIncludProducts;
        }

        public CategoryWithProductsDto GetCategoryById(int id)
        {
            var categoryWithProducts = _unitOfWork.CategoryRepository.GetAll().Include(x => x.Products)
                                        .AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (categoryWithProducts == null)
                return null;

            var categoryWithProductsDto = new CategoryWithProductsDto
            {
                Category = mapperUnitOfWork.Mapper<Category, DtoCategory>().MapEntity(categoryWithProducts),
                Products = categoryWithProducts.Products.Select(i => mapperUnitOfWork.Mapper<Product, DtoProduct>().MapEntity(i)).ToList()
            };

            return categoryWithProductsDto;
        }

        public CategoryWithProductsDto GetCategoryBySlug(string urlSlug)
        {
            var categoryWithProductsDto = this.GetCategories().FirstOrDefault(x => string.Equals(x.Category.UrlSlug, urlSlug, StringComparison.OrdinalIgnoreCase));

            if (categoryWithProductsDto == null)
                return null; // new CategoryWithProductsDto();

            return categoryWithProductsDto;
        }

        public DtoCategory AddCategory(CategoryUploadForm categoryUploadForm)
        {
            Category newCategory = new
                Category {Name = categoryUploadForm.Name, Image = categoryUploadForm.ImageFile.FileName};

            string automaticallyGeneratedSlug = generateUrlSlugClass.GenerateUrlSlug(categoryUploadForm.Name);

            var existCategory = _unitOfWork.CategoryRepository.GetAll().Where(x => x.UrlSlug == automaticallyGeneratedSlug).FirstOrDefault();

            if (existCategory != null)
                return null;

            newCategory.UrlSlug = automaticallyGeneratedSlug;

            _unitOfWork.CategoryRepository.Add(newCategory);
            _unitOfWork.Complete();

            return mapperUnitOfWork.Mapper<Category, DtoCategory>().MapEntity(newCategory);
        }

        public DtoCategory UpdateCategory(DtoCategory dtoCategory)
        {
            var dbCategory = _unitOfWork.CategoryRepository.GetById(dtoCategory.Id);
            if (dbCategory == null)
                return null;

            var newSlug = generateUrlSlugClass.GenerateUrlSlug(dtoCategory.Name);

            var slugExist = _unitOfWork.CategoryRepository.GetAll()
                .FirstOrDefault(x => x.UrlSlug == newSlug);

            if (slugExist != null && slugExist.Id != dbCategory.Id)
            {
                return null;
            }

            mapperUnitOfWork.MapperDtoCategory<DtoCategory, Category>()
                .MapDtoEntityToDistination(dtoCategory, dbCategory);

            dbCategory.UrlSlug = newSlug;

            _unitOfWork.CategoryRepository.Update(dbCategory);
            _unitOfWork.Complete();

            return mapperUnitOfWork.Mapper<Category, DtoCategory>().MapEntity(dbCategory);
        }

        public void DeleteCategory(DtoCategory dtoCategory)
        {
            var dbCategory = _unitOfWork.CategoryRepository.GetById(dtoCategory.Id);
            var category = mapperUnitOfWork.MapperDtoCategory<DtoCategory, Category>().MapDtoEntityToDistination(dtoCategory, dbCategory);
            _unitOfWork.CategoryRepository.Remove(category);
            _unitOfWork.Complete();
        }
    }
}