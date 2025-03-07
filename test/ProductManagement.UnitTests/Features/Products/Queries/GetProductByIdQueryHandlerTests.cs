using System.Data;
using Dapper;
using Moq;
using ProductManagement.Application.Common.Factories;
using ProductManagement.Application.Features.Products.Queries;
using ProductManagement.Shared.Dtos;

namespace ProductManagement.UnitTests.Features.Products.Queries;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IQueryHandlerFactory> _mockFactory;
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _mockFactory = new Mock<IQueryHandlerFactory>();
        _mockDbConnection = new Mock<IDbConnection>();

        // Mock factory to return mock database connection
        _mockFactory.Setup(f => f.CreateConnection()).Returns(_mockDbConnection.Object);

        _handler = new GetProductByIdQueryHandler(_mockFactory.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var expectedProduct = new ProductDto { Id = 1, Name = "Laptop", Price = 1200.50M };

        // Mock QueryFirstOrDefaultAsync to return a product
        _mockDbConnection
            .Setup(conn => conn.QueryFirstOrDefaultAsync<ProductDto>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync(expectedProduct);

        var query = new GetProductByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProduct.Id, result.Id);
        Assert.Equal(expectedProduct.Name, result.Name);
        Assert.Equal(expectedProduct.Price, result.Price);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        _mockDbConnection
            .Setup(conn => conn.QueryFirstOrDefaultAsync<ProductDto>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync((ProductDto)null); // Simulating no product found

        var query = new GetProductByIdQuery(999);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}