using FluentValidation;
using ProductService.WebApi.Controllers.Dao;

namespace ProductService.WebApi.Validators.Asp;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("Name cannot be empty");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than zero");

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid category");

        RuleFor(x => x.CreationDate)
            .NotNull()
            .NotEmpty()
            .MustBeValidDateTime()
            .WithMessage("Invalid creation date");

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .WithMessage("WarehouseId must be greater than zero");
    }
}