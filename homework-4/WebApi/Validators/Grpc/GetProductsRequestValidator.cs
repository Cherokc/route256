using FluentValidation;
using ProductGrpcService;

namespace ProductService.WebApi.Validators.Grpc;

public class GetProductsRequestValidator : AbstractValidator<GetProductsRequest>
{
    public GetProductsRequestValidator()
    {
        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid category")
            .When(x => x.Category != ProductCategory.ProductUnspecified);

        RuleFor(x => x.CreationDate)
            .MustBeValidTimestamp()
            .WithMessage("Invalid creation date")
            .When(x => x.CreationDate != null);  

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .WithMessage("WarehouseId must be greater than zero")
            .When(x => x.WarehouseId != 0);

        RuleFor(x => x.Cursor)
            .Must(cursor => Guid.TryParse(cursor, out _))
            .When(x => x.Cursor != "");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero")
            .When(x => x.PageSize != 0);
    }
}