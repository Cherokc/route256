using FluentValidation;
using ProductGrpcService;

namespace ProductService.WebApi.Validators;

public class ProtoProductValidator : AbstractValidator<Product>
{
    public ProtoProductValidator()
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
            .NotEqual(ProductCategory.ProductUnspecified)
            .WithMessage("Invalid category");

        RuleFor(x => x.CreationDate)
            .NotNull()
            .NotEmpty()
            .WithMessage("Invalid creation date");

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .WithMessage("WarehouseId must be greater than zero");
    }
}