using FluentValidation;
using Google.Protobuf.WellKnownTypes;

namespace ProductService.WebApi.Validators;

public static class TimestampValidationExtensions
{
    public static IRuleBuilderOptions<T, Timestamp> MustBeValidTimestamp<T>(this IRuleBuilder<T, Timestamp> ruleBuilder)
    {
        return ruleBuilder
            .Must(BeAValidTimestamp);
    }

    private static bool BeAValidTimestamp(Timestamp timestamp)
    {
        if (timestamp.Nanos < 0 || timestamp.Nanos > 1000000000)
            return false;
        if (timestamp.Seconds < 0)
            return false;

        var date = timestamp.ToDateTime();
        return date >= DateTime.UnixEpoch && date <= DateTime.UtcNow;
    }
}
