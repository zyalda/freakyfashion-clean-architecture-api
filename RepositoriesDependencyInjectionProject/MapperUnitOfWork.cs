using ApplicationLayer.Interfaces;
using InfrastructureLayer.Mapping;
using Microsoft.Extensions.DependencyInjection; // Required for GetRequiredService
using Microsoft.Identity.Client;
using System.Collections.Concurrent;


namespace InfrastructureLayer.UnitOfWorks
{
    public class MapperUnitOfWork : IMapperUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider; // Built-in .NET interface
        private readonly ConcurrentDictionary<(Type, Type), object> _cache = new();

        // .NET automatically passes the built-in service provider here
        public MapperUnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMapper<TSource, TDestination> Mapper<TSource, TDestination>()
        {
            var key = (typeof(TSource), typeof(TDestination));

            var mapper = _cache.GetOrAdd(key, _ =>
            {
                // Asks the built-in provider to find the registered mapper
                return _serviceProvider.GetRequiredService<IMapper<TSource, TDestination>>();
            });

            return (IMapper<TSource, TDestination>)mapper;
        }

        public IMapperDtoProduct<TSource, TDestination> MapperDtoProduct<TSource, TDestination>()
        {
            var key = (typeof(TSource), typeof(TDestination));

            var mapper = _cache.GetOrAdd(key, _ =>
            {
                // Asks the built-in provider to find the registered mapper
                return _serviceProvider.GetRequiredService<IMapperDtoProduct<TSource, TDestination>>();
            });

            return (IMapperDtoProduct<TSource, TDestination>)mapper;
        }

        public IMapperDtoCategory<TSource, TDestination> MapperDtoCategory<TSource, TDestination>()
        {
            var key = (typeof(TSource), typeof(TDestination));

            var mapper = _cache.GetOrAdd(key, _ =>
            {
                // Asks the built-in provider to find the registered mapper
                return _serviceProvider.GetRequiredService<IMapperDtoCategory<TSource, TDestination>>();
            });

            return (IMapperDtoCategory<TSource, TDestination>)mapper;
        }
        public IMapperDtoOrder<TSource, TDestination> MapperDtoOrder<TSource, TDestination>()
        {
            var key = (typeof(TSource), typeof(TDestination));

            var mapper = _cache.GetOrAdd(key, _ =>
            {
                // Asks the built-in provider to find the registered mapper
                return _serviceProvider.GetRequiredService<IMapperDtoOrder<TSource, TDestination>>();
            });

            return (IMapperDtoOrder<TSource, TDestination>)mapper;
        }
        public IMapperDtoOrderItem<TSource, TDestination> MapperDtoOrderItem<TSource, TDestination>()
        {
            var key = (typeof(TSource), typeof(TDestination));

            var mapper = _cache.GetOrAdd(key, _ =>
            {
                // Asks the built-in provider to find the registered mapper
                return _serviceProvider.GetRequiredService<IMapperDtoOrderItem<TSource, TDestination>>();
            });

            return (IMapperDtoOrderItem<TSource, TDestination>)mapper;
        }
    }
}
