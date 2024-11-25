using FluentValidation;
using ProductGrpcService;

namespace ProductService.WebApi.Validators.Grpc;

public class UpdateProductPriceRequestValidator : AbstractValidator<UpdateProductPriceRequest>
{
    public UpdateProductPriceRequestValidator()
    {
        RuleFor(x => x.NewPrice)
            .GreaterThan(0)
            .WithMessage("NewPrice must be greater than zero");
    }
}