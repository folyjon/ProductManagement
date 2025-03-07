using MediatR;
using ProductManagement.Shared.Dtos;

namespace ProductManagement.Application.Features.Products.Queries
{
    public record GetProductsQuery(int PageNumber, int PageSize) : IRequest<PaginatedResult<ProductDto>>;
}