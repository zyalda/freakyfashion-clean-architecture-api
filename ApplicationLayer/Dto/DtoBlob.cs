namespace ApplicationLayer.Dto
{
    public class DtoBlob
    {
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Stream { get; set; }
    }
}
