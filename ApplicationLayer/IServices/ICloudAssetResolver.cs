using ApplicationLayer.IStorageContainerServices;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.IServices
{
    public interface ICloudAssetResolver
    {
        Task ResolveSingleAsync<T>(T item) where T : class, IHasCloudImage;

        Task ResolveCollectionAsync<T>(IEnumerable<T> items) where T : class, IHasCloudImage;
    }
}
