using FluentValidation;
using ProductService.WebApi.Controllers.Dao;

namespace ProductService.WebApi.Validators.Asp;

public class UpdatePriceRequestValidator : AbstractValidator<UpdatePriceRequest>
{
    public UpdatePriceRequestValidator()
    {
        RuleFor(x => x.NewPrice)
            .GreaterThan(0)
            .WithMessage("NewPrice must be greater than zero");
    }
}