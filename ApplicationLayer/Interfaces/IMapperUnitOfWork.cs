namespace ApplicationLayer.Interfaces
{
    public interface IMapperUnitOfWork
    {
        IMapper<TSource, TDestination> Mapper<TSource, TDestination>();
        IMapperDtoProduct<TSource, TDestination> MapperDtoProduct<TSource, TDestination>();
        IMapperDtoCategory<TSource, TDestination> MapperDtoCategory<TSource, TDestination>();
        IMapperDtoOrder<TSource, TDestination> MapperDtoOrder<TSource, TDestination>();
        IMapperDtoOrderItem<TSource, TDestination> MapperDtoOrderItem<TSource, TDestination>();
    }
}
