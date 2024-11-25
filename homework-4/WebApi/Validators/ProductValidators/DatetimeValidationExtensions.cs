using FluentValidation;

namespace ProductService.WebApi.Validators;

public static class DatetimeValidationExtensions
{
    public static IRuleBuilderOptions<T, DateTime> MustBeValidDateTime<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder
            .Must(BeAValidDateTime);
    }

    private static bool BeAValidDateTime(DateTime datetime)
    {
        return datetime != DateTime.MinValue && datetime <= DateTime.Now;
    }
}
