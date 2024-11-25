using FluentValidation;
using ProductGrpcService;

namespace ProductService.WebApi.Validators.Grpc;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
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
            .MustBeValidTimestamp()
            .WithMessage("Invalid creation date");

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .WithMessage("WarehouseId must be greater than zero");
    }
}