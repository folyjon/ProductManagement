using MediatR;
using ProductManagement.Shared.Dtos;

namespace ProductManagement.Application.Features.Products.Queries
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
}
