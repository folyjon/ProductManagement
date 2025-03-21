using Azure.Storage.Blobs;
using ProductManagement.Application.Interfaces.Services;

namespace ProductManagement.Infrastructure.Services
{
    public class BlobStorageService(BlobServiceClient blobServiceClient) : IStorageService
    {
        public async Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteFileAsync(string containerName, string fileName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}