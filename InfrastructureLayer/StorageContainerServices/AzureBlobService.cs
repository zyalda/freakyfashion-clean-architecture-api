using ApplicationLayer.Dto;
using ApplicationLayer.IStorageContainerServices;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace InfrastructureLayer.StorageContainerServices
{
    public class AzureBlobService: IAzureBlobService
    {
        private readonly BlobServiceClient _serviceClient;
        private readonly string _storageAccountName;
        private readonly string _containerName;
        private readonly string _containerOrderName;
        private readonly string containerName = string.Empty;
        private readonly DefaultAzureCredential _defaultAzureCredential;

        public AzureBlobService(IOptions<AzureBlobSettings> settings)
        {
            _storageAccountName = settings.Value.StorageName;
            _containerName = settings.Value.ContainerName;
            _containerOrderName = settings.Value.OrdersContainerName;
            _defaultAzureCredential = new DefaultAzureCredential();

            // Construct the blob service endpoint URI
            //var uri = new Uri($"https://{_storageAccountName}.blob.core.windows.net");

            // Use DefaultAzureCredential to authenticate with Managed Identity.
            //_serviceClient = new BlobServiceClient(uri, defaultAzureCredential);
            //Goes to Azure Blob on life mode or to Azurite on local mode.
            _serviceClient = new BlobServiceClient("UseDevelopmentStorage=true");
        }
        public async Task<DtoBlob> UploadBlobAsync(IFormFile file)
        {
            DtoBlob dtoBlob = new DtoBlob();
            var containerClient = _serviceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            string fileName = file.FileName;
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // OpenReadStream to stream data directly to Azure
            using (Stream? stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream);
            }
            dtoBlob.Name = fileName;
            dtoBlob.Uri = blobClient.Uri.AbsoluteUri;

            return dtoBlob;
        }

        public async Task<DtoBlob> DownloadBlobAsync(string fileName)
        {
            var containerClient = _serviceClient.GetBlobContainerClient(containerName);
            BlobClient file = containerClient.GetBlobClient(fileName);

            if (await file.ExistsAsync())
            {
                var data = await file.OpenReadAsync();
                Stream stream = data;

                var content = await file.DownloadContentAsync();

                string name = file.Name;
                string contentType = content.Value.Details.ContentType;

                return new DtoBlob { Name = name, ContentType = contentType, Stream = stream, Uri = file.Uri.AbsoluteUri };
            }
            return new DtoBlob();
        }

        public async Task<string> DownloadBlobSasUrl(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;

            //Validate user permissions here via standard API authorization
            var containerClient = _serviceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            if (!blobClient.Exists()) return "Not found";

            //Create a SAS (Shared Access Signature) token that only permits "Read" and expires in 10 minutes
            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    BlobName = blobClient.Name,
                    Resource = "b", // "b" stands for blob
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                //Generate the absolute URL with the signature appended
                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

                // Return the URL as a string response to React
                return sasUri.ToString();
            }
            return blobClient.Uri.ToString();
        }

        public async Task<string> UploadBlobAsync(string jsonContent, string orderNumber)
        {
            string fileName = $"order-{orderNumber}.json";

            var containerClient = _serviceClient.GetBlobContainerClient(_containerOrderName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(fileName);

            // Azure sparar här metadata (information om filen som ska skapas i nästa steg).
            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "application/json" }
            };

            await blobClient.UploadAsync(BinaryData.FromString(jsonContent), options);
            
            return fileName;
        }
    }
}
