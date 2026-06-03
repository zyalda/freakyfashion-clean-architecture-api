using ApplicationLayer.IStorageContainerServices;

namespace ApplicationLayer.Dto
{
    public class DtoCategory : IHasCloudImage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string UrlSlug { get; set; } = string.Empty;
         public string StatusMessage {  get; set; } = string.Empty;
    }
}
