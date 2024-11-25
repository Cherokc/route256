using FluentValidation;
using ProductGrpcService;

namespace ProductService.WebApi.Validators.Grpc;

public class GetProductByIdRequestValidator : AbstractValidator<GetProductByIdRequest>
{
    public GetProductByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .NotEmpty()
            .WithMessage("Id cannot be empty");
    }
}