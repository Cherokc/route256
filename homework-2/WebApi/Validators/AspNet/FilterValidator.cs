using FluentValidation;
using WebApi.Controllers.Dao;
using WebApi.Validators;
using ProductService.Domain.Dao;

namespace ProductService.WebApi.Validators.Asp;

public class FilterValidator : AbstractValidator<Filter>
{
    public FilterValidator()
    {
        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid category")
            .When(x => x.Category != ProductCategory.Unspecified);

        RuleFor(x => x.CreationDate)
            .MustBeValidDateTime()
            .WithMessage("Invalid creation date")
            .When(x => x.CreationDate != DateTime.MinValue);

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .WithMessage("WarehouseId must be greater than zero")
            .When(x => x.WarehouseId != 0);

        RuleFor(x => x.Cursor)
            .NotNull()
            .NotEmpty()
            .WithMessage("Cursor must be presented as GUID")
            .When(x => x.Cursor != default(Guid));

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero")
            .When(x => x.PageSize != 0);
    }
}