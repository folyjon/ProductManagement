using FluentValidation.TestHelper;
using ProductManagement.Application.Features.Products.Commands;
using ProductManagement.Application.Validators;
using Xunit;

namespace ProductManagement.UnitTests.Validators;

public class ProductValidatorTests
{
    private readonly ProductValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_Name_Is_Empty(string invalidName)
    {
        // Arrange
        var model = new AddProductCommand { Name = invalidName };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Name);
    }

    [Theory]
    [InlineData(101)] // Boundary Case: Exceeds max length
    [InlineData(200)]
    public void Should_Have_Error_When_Name_Exceeds_Max_Length(int length)
    {
        var model = new AddProductCommand { Name = new string('A', length) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(p => p.Name);
    }

    [Theory]
    [InlineData(501)] // Boundary Case: Exceeds max length
    [InlineData(600)]
    public void Should_Have_Error_When_Description_Exceeds_Max_Length(int length)
    {
        var model = new AddProductCommand { Description = new string('A', length) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(p => p.Description);
    }

    [Theory]
    [InlineData(0)]   // Invalid: Zero
    [InlineData(-10)] // Invalid: Negative
    public void Should_Have_Error_When_Price_Is_Not_Greater_Than_Zero(decimal invalidPrice)
    {
        var model = new AddProductCommand { Price = invalidPrice };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(p => p.Price);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var model = new AddProductCommand
        {
            Name = "Valid Product",
            Description = "A valid product description",
            Price = 99.99M
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(p => p.Name);
        result.ShouldNotHaveValidationErrorFor(p => p.Description);
        result.ShouldNotHaveValidationErrorFor(p => p.Price);
    }
}