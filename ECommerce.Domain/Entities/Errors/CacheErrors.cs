using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities.Errors;

public static class CacheErrors
{
    public static readonly Error OperationFailed = 
        Error.Unavailable(
            "Cache.OperationFailed", 
            "A caching service is temporarily unavailable. Please try again later.");
}
