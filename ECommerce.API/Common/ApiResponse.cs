namespace ECommerce.API.Common;

/// <summary>
/// The single envelope shape for every SUCCESSFUL response in the API.
/// "T" is a single object (e.g. one Product) OR a collection (e.g. IReadOnlyList&lt;Product&gt;) —
/// the envelope shape stays identical either way, only the "data" field's contents differ.
/// </summary>
public sealed record ApiResponse<T>(
    bool Success, string? Message, T? Data, ApiMeta Meta);
