namespace ApplicationLayer.Interfaces
{
    public interface IMapper<TSource, TDestination>
    {
        public TDestination MapEntity(TSource source);
    }
}
