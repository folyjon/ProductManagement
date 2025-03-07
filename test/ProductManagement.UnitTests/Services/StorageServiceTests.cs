using Moq;
using ProductManagement.Application.Interfaces.Services;

namespace ProductManagement.UnitTests.Services;

public class StorageServiceTests
{
    private readonly Mock<IStorageService> _mockStorageService = new();

    [Fact]
    public async Task UploadFileAsync_ReturnsBlobUri()
    {
        // Arrange
        var expectedUri = "https://storageaccount.blob.core.windows.net/container/blob";
        _mockStorageService
            .Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(expectedUri);

        // Act
        var result = await _mockStorageService.Object.UploadFileAsync("container", "blob", new MemoryStream());

        // Assert
        Assert.Equal(expectedUri, result);
    }
}