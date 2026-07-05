using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Enums;

namespace ECommerce.API.Extensions;

/// <summary>
/// Converts a Result/ResultOfT<T> (domain-facing) into an IResult (HTTP-facing).
/// This is the ONE place that knows how ResultTypes/ErrorTypes map to HTTP status codes —
/// every endpoint reuses this instead of re-writing the switch statement.
/// </summary>
public static class ResultExtensions
{
    // For non-generic Result (e.g. commands with no return value)
    public static IResult ToApiResult(this Domain.Common.Result result)
    {
        if (result.IsSuccess)
        {
            return result.ResultType switch
            {
                ResultTypes.Created => Results.Created(),
                ResultTypes.NoContent => Results.NoContent(),
                _ => Results.Ok()
            };
        }

        return MapError(result.Error!);
    }

    // For ResultOfT<T> (e.g. queries that return data)
    public static IResult ToApiResult<T>(this ResultOfT<T> result, string? locationRoute = null)
    {
        if (result.IsSuccess)
        {
            return result.ResultType switch
            {
                ResultTypes.Created => Results.Created(locationRoute ?? string.Empty, result.Value),
                ResultTypes.NoContent => Results.NoContent(),
                _ => Results.Ok(result.Value)
            };
        }

        return MapError(result.Error!);
    }

    // Shared failure mapping — the ONLY place ErrorTypes -> HTTP status is decided
    private static IResult MapError(Error error) => error.Type switch
    {
        ErrorTypes.Validation => Results.BadRequest(new { error.Code, error.Message }),
        ErrorTypes.NotFound => Results.NotFound(new { error.Code, error.Message }),
        ErrorTypes.Conflict => Results.Conflict(new { error.Code, error.Message }),
        ErrorTypes.UnAuthorized => Results.Unauthorized(),
        ErrorTypes.Forbidden => Results.Forbid(),
        _ => Results.Problem(
            title: "Internal Server Error",
            detail: "An error occurred. Please try again later.",
            statusCode: 500)
    };
}
