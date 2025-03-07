using System.Data;
using Dapper;
using MediatR;
using ProductManagement.Application.Common.Factories;
using ProductManagement.Shared.Dtos;

namespace ProductManagement.Application.Features.Products.Queries;

public class GetProductsQueryHandler(IQueryHandlerFactory queryHandlerFactory)
    : IRequestHandler<GetProductsQuery, PaginatedResult<ProductDto>>
{
    public async Task<PaginatedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        using var connection = queryHandlerFactory.CreateConnection();

        const string sql = @"
            WITH PaginatedProducts AS
            (
                SELECT
                    Id,
                    Name,
                    Price,
                    Description,
                    ImageUrl,
                    COUNT(*) OVER() AS TotalCount
                FROM Products
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY
            )
            SELECT Id, Name, Price, Description, ImageUrl, TotalCount FROM PaginatedProducts;
        ";

        var offset = (request.PageNumber - 1) * request.PageSize;
        var resultCount = 0;
        
        var products = await connection.QueryAsync<ProductDto, int, ProductDto>(
            sql,
            (product, totalCount) =>
            {
                resultCount = totalCount;
                
                return product;
            },
            new { Offset = offset, request.PageSize },
            splitOn: "TotalCount"
        );

        return new PaginatedResult<ProductDto>(products, resultCount, request.PageNumber, request.PageSize);
    }
}