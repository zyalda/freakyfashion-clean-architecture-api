using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Dto
{
    public class ProductUploadForm
    {
        //public DtoProduct DtoProductData { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        //public DtoCategory DtoCategoryData { get; set; } = null!;
        public IFormFile ImageFile { get; set; } = null!;
    }
}
