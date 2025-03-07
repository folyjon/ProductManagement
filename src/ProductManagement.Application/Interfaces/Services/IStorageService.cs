namespace ProductManagement.Application.Interfaces.Services;

public interface IStorageService
{
    Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream);
    Task DeleteFileAsync(string containerName, string fileName);
}