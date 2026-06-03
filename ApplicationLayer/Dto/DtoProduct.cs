using ApplicationLayer.IStorageContainerServices;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Dto
{
    public class DtoProduct : IHasCloudImage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Image { get; set; } = string.Empty;
        public string UrlSlug { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
        public bool IsAdded { get; set; } = true;
    }
}
