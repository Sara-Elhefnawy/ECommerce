using Microsoft.Extensions.Options;

namespace ECommerce.APP.Cachings;

public sealed class CacheEntryPolicyValidator : IValidateOptions<CacheEntryPolicy>
{
    public ValidateOptionsResult Validate(string? name, CacheEntryPolicy options)
    {
        if (options.AbsoluteExpirationDays <= 0)
            return ValidateOptionsResult.Fail(
                "AbsoluteExpirationDays must be greater than 0.");

        if (options.SlidingExpirationDays <= 0)
            return ValidateOptionsResult.Fail(
                "SlidingExpirationDays must be greater than 0.");

        if (options.SlidingExpirationDays > options.AbsoluteExpirationDays)
            return ValidateOptionsResult.Fail(
                "SlidingExpirationDays cannot exceed AbsoluteExpirationDays.");

        if (options.SlidingRefreshThresholdMinutes <= 0)
            return ValidateOptionsResult.Fail(
                "SlidingRefreshThresholdMinutes must be greater than 0.");

        if (options.SlidingRefreshThresholdMinutes >
            options.SlidingExpirationDays * 24 * 60)
            return ValidateOptionsResult.Fail(
                "Sliding refresh threshold cannot exceed sliding expiration.");

        if (options.LocalCacheExpirationMinutes <= 0)
            return ValidateOptionsResult.Fail(
                "LocalCacheExpirationMinutes must be greater than 0.");

        return ValidateOptionsResult.Success;
    }
}
