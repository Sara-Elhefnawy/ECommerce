namespace ECommerce.API.Common;

/// <summary>
/// Metadata attached to every success response.
/// Pagination is optional — only present when the endpoint actually paginates,
/// omitted (null) otherwise so it doesn't clutter simple list responses.
/// </summary>
public sealed record ApiMeta(
    string TraceId, PaginationMeta? Pagination = null);
