namespace ECommerce.APP.Cachings;

public class CacheEntryPolicy
{
    public int AbsoluteExpirationDays { get; set; } = 30;

    public int SlidingExpirationDays { get; set; } = 7;

    public int LocalCacheExpirationMinutes { get; set; } = 5;

    // is for not everytime the user (refresh the page, got out then in the page,...)
    //      then i update redis of 
    // so you hold 30 mintues before updating redis
    // only update LastAccessedUtc after the 30 minutes of SlidingRefreshThresholdMinutes
    // without this it will go to redis alot and that affect performance
    public int SlidingRefreshThresholdMinutes { get; set; } = 30;
}
