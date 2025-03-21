using MediatR;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces.Repositories;

namespace ProductManagement.Application.Features.Products.Commands
{
    public class AddProductCommandHandler(IProductRepository productRepository)
        : IRequestHandler<AddProductCommand, int>
    {
        public async Task<int> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl
            };

            await productRepository.AddAsync(product);
            await productRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            
            return product.Id;
        }
    }
}
