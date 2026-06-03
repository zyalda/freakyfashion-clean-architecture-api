using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using ApplicationLayer.IServices;
using DomainLayer.Entites;

namespace ApplicationLayer.Services
{
    public class ProductService :IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenerateUrlSlugClass generateUrlSlugClass;
        private readonly IMapperUnitOfWork mapperUnitOfWork;

        public ProductService(IUnitOfWork unitOfWork, IMapperUnitOfWork mapperUnitOfWork, IGenerateUrlSlugClass generateUrlSlugClass)
        {
            _unitOfWork = unitOfWork;
            this.mapperUnitOfWork = mapperUnitOfWork;
            this.generateUrlSlugClass = generateUrlSlugClass;
        }

        public IEnumerable<DtoProduct> GetProducts()
        {
            var products = _unitOfWork.ProductRepository.GetAll();
            
            if (products == null)
                return new List<DtoProduct>();

            var mappers = mapperUnitOfWork.Mapper<Product, DtoProduct>();
            return products.Select(x => mappers.MapEntity(x));
        }

        public DtoProduct GetProductById(int id)
        {
            var product = _unitOfWork.ProductRepository.GetById(id);
            
            if (product == null)
                return null;
            
            return mapperUnitOfWork.Mapper<Product, DtoProduct>().MapEntity(product);
        }

        public DtoProduct GetProductBySlug(string slug)
        {
            var product = this.GetProducts().FirstOrDefault(x => string.Equals(x.UrlSlug, slug, StringComparison.OrdinalIgnoreCase));

            if (product == null)
                return null;

            return product;
        }

        public DtoProduct AddProduct(ProductUploadForm productUploadForm)
        {
            var dbcategory = _unitOfWork.CategoryRepository.GetAll().Where(x => x.Name.ToLower() == productUploadForm.CategoryName.ToLower()).SingleOrDefault();

            var imageFileName = productUploadForm.ImageFile.FileName;

            if (dbcategory == null)
            {
                //Generate a urlSlug of input dtoCategory.Name.
                string categoryGeneratedSlug = generateUrlSlugClass.GenerateUrlSlug(productUploadForm.CategoryName);

                var slugForCategoryExisted = _unitOfWork.CategoryRepository.GetAll().Any(x => x.UrlSlug == categoryGeneratedSlug);

                if (slugForCategoryExisted)
                    return new DtoProduct { StatusMessage = $"404 The category name exist. Choose a new name.", IsAdded = false };

                Category category = new Category
                {
                    Name = productUploadForm.CategoryName,
                    Image = imageFileName,
                    UrlSlug = categoryGeneratedSlug
                };

                _unitOfWork.CategoryRepository.Add(category);
                _unitOfWork.Complete();
                dbcategory = category;
            }

            //Generate a urlSlug of input dtoCategory.Name.
            string automaticallyGeneratedSlug = generateUrlSlugClass.GenerateUrlSlug(productUploadForm.Name);

            var slugExiste = _unitOfWork.ProductRepository.GetAll().Any(x => x.UrlSlug == automaticallyGeneratedSlug);

            if (slugExiste)
                return new DtoProduct { StatusMessage = $"404 The product name exist. Choose a new name." , IsAdded = false};

            var product = new Product
            {
                Name = productUploadForm.Name,
                Description = productUploadForm.Description,
                Image = imageFileName,
                UrlSlug = automaticallyGeneratedSlug,
                Price = productUploadForm.Price
            };

            product.CategoryId = dbcategory.Id;

            _unitOfWork.ProductRepository.Add(product);
            _unitOfWork.Complete();

            return mapperUnitOfWork.Mapper<Product, DtoProduct>().MapEntity(product);
        }

        public DtoProduct UpdateProduct(DtoProduct dtoProduct)
        {
            var productDb = _unitOfWork.ProductRepository.GetById(dtoProduct.Id);

            if (productDb == null)
                return null;

            if (dtoProduct.Price <= 0)
                return null;

            var newSlug = generateUrlSlugClass.GenerateUrlSlug(dtoProduct.Name);

            var slugExist = _unitOfWork.ProductRepository.GetAll()
                .FirstOrDefault(x => x.UrlSlug == newSlug);

            if (slugExist != null && slugExist.Id != productDb.Id)
            {
                return null;
            }

            mapperUnitOfWork.MapperDtoProduct<DtoProduct, Product>().MapDtoEntityToDistination(dtoProduct, productDb);

            productDb.UrlSlug = newSlug;
            _unitOfWork.ProductRepository.Update(productDb);
            _unitOfWork.Complete();
            return mapperUnitOfWork.Mapper<Product, DtoProduct>().MapEntity(productDb);
        }

        public void DeleteProduct(DtoProduct dtoProduct)
        {
            var productDb = _unitOfWork.ProductRepository.GetById(dtoProduct.Id);
            var product = mapperUnitOfWork.MapperDtoProduct<DtoProduct, Product>().MapDtoEntityToDistination(dtoProduct, productDb);
            _unitOfWork.ProductRepository.Remove(product);
            _unitOfWork.Complete();
        }
    }
}
