using FluentValidation;
using ProductManagement.Application.Features.Products.Commands;

namespace ProductManagement.Application.Validators;

public class ProductValidator : AbstractValidator<AddProductCommand>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Description).MaximumLength(500);
        RuleFor(p => p.Price).GreaterThan(0);
    }
}