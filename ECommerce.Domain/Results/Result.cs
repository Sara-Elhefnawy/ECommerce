using ECommerce.Domain.Entities.Enums;

namespace ECommerce.Domain.Results;

/// <summary>
/// The Result Pattern - A wrapper that explicitly represents success or failure of an operation
/// This eliminates exceptions for expected failures and makes error handling explicit
/// </summary>
public abstract class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ResultTypes ResultType { get; }
    public Error? Error { get; }


    // Protected constructor to enforce creation through factory methods
    protected Result(bool isSuccess, ResultTypes type, Error? error = null)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Success cannot have error");

        if (!isSuccess && error is null)
            throw new InvalidOperationException("Failure must have error");

        IsSuccess = isSuccess;
        ResultType = type;
        Error = error;
    }

    // Factory Methods for Non-Generic Results
    public static Result Ok() => new SuccessResult(ResultTypes.Ok);
    public static Result Created() => new SuccessResult(ResultTypes.Created);
    public static Result NoContent() => new SuccessResult(ResultTypes.NoContent);
    public static Result BadRequest(Error error) => new FailureResult(error, ResultTypes.BadRequest);
    public static Result NotFound(Error error) => new FailureResult(error, ResultTypes.NotFound);
    public static Result Conflict(Error error) => new FailureResult(error, ResultTypes.Conflict);
    public static Result Unauthorized(Error error) => new FailureResult(error, ResultTypes.Unauthorized);
    public static Result Forbidden(Error error) => new FailureResult(error, ResultTypes.Forbidden);
    public static Result Failure(Error error) => new FailureResult(error, ResultTypes.BadRequest); // Maps to 500
}

// Concrete Result Classes
public sealed class SuccessResult(ResultTypes type) : Result(true, type)
{
}

public sealed class FailureResult(Error error, ResultTypes type) : Result(false, type, error)
{
}
