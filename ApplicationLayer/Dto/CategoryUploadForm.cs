using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Dto
{
    public class CategoryUploadForm
    {
        //public DtoCategory DtoCategoryData { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public IFormFile ImageFile { get; set; } = null!;
    }
}
