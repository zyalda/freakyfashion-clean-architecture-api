using ApplicationLayer.Dto;

namespace ApplicationLayer.IServices
{
    public interface IProductService
    {
        public IEnumerable<DtoProduct> GetProducts();
        public DtoProduct GetProductById(int id);
        public DtoProduct GetProductBySlug(string slug);
        public DtoProduct AddProduct(ProductUploadForm productUploadForm);
        public DtoProduct UpdateProduct(DtoProduct dtoProduct);
        public void DeleteProduct(DtoProduct dtoProduct);
    }
}
