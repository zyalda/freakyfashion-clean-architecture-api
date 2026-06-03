using ApplicationLayer.IServices;
using ApplicationLayer.IStorageContainerServices;

namespace ApplicationLayer.Services
{
    public class CloudAssetResolver : ICloudAssetResolver
    {
        private readonly IAzureBlobService azureBlobService;

        public CloudAssetResolver(IAzureBlobService azureBlobService)
        {
            this.azureBlobService = azureBlobService;
        }

        public async Task ResolveSingleAsync<T>(T item) where T : class, IHasCloudImage
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Image)) return;

            try
            {
                string secureUrl = await azureBlobService.DownloadBlobSasUrl(item.Image);

                if (secureUrl != "Not found" && !string.IsNullOrWhiteSpace(secureUrl))
                {
                    // Assign the secure temporary image url path.
                    item.Image = secureUrl;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to map SAS token for asset: {item.Image}. Error: {ex.Message}");
            }
        }

        //public async Task ResolveSingleAsync<T>(T item) where T : class, IHasCloudImage
        //{
        //    if (item == null || string.IsNullOrWhiteSpace(item.Image)) return;

        //    var blobData = await azureBlobService.DownloadBlobAsync(item.Image);

        //    if (blobData.Stream != null)
        //    {
        //        item.Image = blobData.Uri ?? item.Image;

        //        blobData.Stream.Close();
        //        await blobData.Stream.DisposeAsync();
        //    }
        //}

        public async Task ResolveCollectionAsync<T>(IEnumerable<T> items) where T : class, IHasCloudImage
        {
            if (items == null || !items.Any()) return;

            foreach (var item in items)
            {
                await ResolveSingleAsync(item);
            }
        }
    }
}
