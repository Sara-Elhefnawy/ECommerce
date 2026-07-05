namespace ECommerce.API.Common;

/// <summary>
/// Pagination details — only used when Meta.Pagination is set.
/// </summary>
public sealed record PaginationMeta(
    int Page, int PageSize, int TotalCount, int TotalPages);
