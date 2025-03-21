using Dapper;
using MediatR;
using ProductManagement.Application.Common.Factories;
using ProductManagement.Shared.Dtos;

namespace ProductManagement.Application.Features.Products.Queries;

public class GetProductByIdQueryHandler(IQueryHandlerFactory queryHandlerFactory)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = queryHandlerFactory.CreateConnection();

        const string sql = $@"
            SELECT 
                Id, 
                Name, 
                Price, 
                Description, 
                ImageUrl
            FROM Products
            WHERE Id = @Id;
        ";

        var product = await connection.QueryFirstOrDefaultAsync<ProductDto>(sql, new { request.Id });

        return product ?? throw new KeyNotFoundException($"Product with ID {request.Id} not found.");
    }
}