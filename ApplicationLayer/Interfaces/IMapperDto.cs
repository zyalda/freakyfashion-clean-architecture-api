namespace ApplicationLayer.Interfaces
{
    public interface IMapperDtoEntityToDistination<TSource, TDestination>
    {
        public TDestination MapDtoEntityToDistination(TSource source, TDestination destination);
    }

    public interface IMapperDtoProduct<TSource, TDestination>: IMapperDtoEntityToDistination<TSource, TDestination>
    {
        public TDestination MapEntityByParameters(string name, string description, string image, string urlSlug, int price);
    }

    public interface IMapperDtoCategory<TSource, TDestination>: IMapperDtoEntityToDistination<TSource, TDestination>
    {
        public TDestination MapEntityByParameters(string name, string image, string urlSlug);
    }
    public interface IMapperDtoOrder<TSource, TDestination> : IMapperDtoEntityToDistination<TSource, TDestination>
    {
        public TDestination MapEntityByParameters(int TheTotal, int CustomerId);
    }
    public interface IMapperDtoOrderItem<TSource, TDestination> : IMapperDtoEntityToDistination<TSource, TDestination>
    {
        public TDestination MapEntityByParameters(int orderId, int customerId, int quantity, int unitPrice);
    }
}
