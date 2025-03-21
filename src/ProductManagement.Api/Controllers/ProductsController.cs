using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Features.Products.Commands;
using ProductManagement.Application.Features.Products.Queries;
using ProductManagement.Shared.Dtos;

namespace ProductManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ProductDto>>> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await mediator.Send(new GetProductsQuery(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await mediator.Send(new GetProductByIdQuery(id));
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Add([FromBody] AddProductCommand command)
        {
            var productId = await mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = productId }, null);
        }
    }
}
