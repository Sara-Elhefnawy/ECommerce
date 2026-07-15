using ECommerce.API.Common;
using ECommerce.APP.Features.Products.Queries.GetPagination.Constants;
using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Enums;

namespace ECommerce.API.Result;

/// <summary>
/// Converts a Result/ResultOfT<T> (domain-facing) into an IResult (HTTP-facing).
/// This is the ONE place that knows how ResultTypes/ErrorTypes map to HTTP status codes —
///     every endpoint reuses this instead of re-writing the switch statement.
/// </summary>
public static class ResultExtensions
{
    // For non-generic Result (e.g. commands with no return value)
    public static IResult ToApiResult(
        this Domain.Common.Result result,
        HttpContext httpContext,
        string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            var response = new ApiResponse<object?>(
                Success: true,
                Message: successMessage,
                Data: null,
                Meta: new ApiMeta(httpContext.TraceIdentifier));

            return result.ResultType switch
            {
                ResultTypes.Created => Results.Created(),
                ResultTypes.NoContent => Results.NoContent(),
                _ => Results.Ok(response)
            };
        }

        return MapError(result.Error!);
    }

    // For ResultOfT<T> (e.g. queries that return data)
    public static IResult ToApiResult<T>(
        this ResultOfT<T> result,
        HttpContext httpContext,
        string? locationRoute = null,
        string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            var response = new ApiResponse<T>(
                Success: true,
                Message: successMessage,
                Data: result.Value,
                Meta: new ApiMeta(httpContext.TraceIdentifier));

            return result.ResultType switch
            {
                ResultTypes.Created => Results.Created(locationRoute ?? string.Empty, response),
                ResultTypes.NoContent => Results.NoContent(),
                _ => Results.Ok(response)
            };
        }

        return MapError(result.Error!);
    }

    // For paginated results - FIXED: proper generic parameter
    public static IResult ToPaginatedApiResult<T>(
        this ResultOfT<PagedResult<T>> result,
        HttpContext httpContext,
        int? pageNumber,
        int? pageSize,
        string? locationRoute = null,
        string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            var response = new ApiResponse<IReadOnlyList<T>>(
                Success: true,
                Message: successMessage,
                Data: result.Value.Items,
                Meta: new ApiMeta(
                    httpContext.TraceIdentifier,
                    new PaginationMeta(
                        pageNumber ?? ValidatorsConstant.DefaultPageNumber,
                        pageSize ?? ValidatorsConstant.DefaultPageSize,
                        result.Value.TotalCount)));

            return result.ResultType switch
            {
                ResultTypes.Created => Results.Created(locationRoute ?? string.Empty, response),
                ResultTypes.NoContent => Results.NoContent(),
                _ => Results.Ok(response)
            };
        }

        return MapError(result.Error!);
    }

    private static IResult MapError(Error error)
    {
        if (error.Type == ErrorTypes.Validation)
        {
            var errors = new Dictionary<string, string[]>
            {
                [error.Code] = [error.Message]
            };

            return Results.ValidationProblem(
                errors: errors,
                title: "Validation Failed",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var (status, title) = error.Type switch
        {
            ErrorTypes.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
            ErrorTypes.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            ErrorTypes.UnAuthorized => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ErrorTypes.Forbidden => (StatusCodes.Status403Forbidden, "Forbidden"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        return Results.Problem(
            detail: error.Message,
            statusCode: status,
            title: title);
    }
}
