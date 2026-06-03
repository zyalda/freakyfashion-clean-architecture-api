namespace ApplicationLayer.Dto
{
    public class CategoryWithProductsDto
    {
        public DtoCategory Category { get; set; } = null!;
        public IEnumerable<DtoProduct> Products { get; set; } = Enumerable.Empty<DtoProduct>();
    }
}