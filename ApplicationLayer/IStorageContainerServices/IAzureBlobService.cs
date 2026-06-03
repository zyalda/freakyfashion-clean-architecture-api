using ApplicationLayer.Dto;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.IStorageContainerServices
{
    public interface IAzureBlobService
    {
        Task<DtoBlob> UploadBlobAsync(IFormFile file);
        Task<DtoBlob> DownloadBlobAsync(string fileName);
        Task<string> DownloadBlobSasUrl(string blobName);
    }
}
