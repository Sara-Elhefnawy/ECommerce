using ECommerce.Domain.Common.Enums;

namespace ECommerce.Domain.Common;

public sealed class ResultOfT<T> : Result
{
    private readonly T? _value;

    private ResultOfT(T? value, bool isSuccess, ResultTypes type, Error? error = null)
        : base(isSuccess, type, error)
    {
        _value = value;
    }

    // Gets the value if successful, throws if failed
    // This forces developers to check IsSuccess before accessing Value
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of failed result");

    // Factory Methods for Generic Results
    public static ResultOfT<T> Ok(T value) => new(value, true, ResultTypes.Ok);
    public static ResultOfT<T> Created(T value) => new(value, true, ResultTypes.Created);
    public new static ResultOfT<T> NoContent() => new(default, true, ResultTypes.NoContent);
    public new static ResultOfT<T> BadRequest(Error error) => new(default, false, ResultTypes.BadRequest, error);
    public new static ResultOfT<T> NotFound(Error error) => new(default, false, ResultTypes.NotFound, error);
    public new static ResultOfT<T> Conflict(Error error) => new(default, false, ResultTypes.Conflict, error);
    public new static ResultOfT<T> Unauthorized(Error error) => new(default, false, ResultTypes.Unauthorized, error);
    public new static ResultOfT<T> Forbidden(Error error) => new(default, false, ResultTypes.Forbidden, error);
    public new static ResultOfT<T> Failure(Error error) => new(default, false, ResultTypes.BadRequest, error);

    // Implicit conversions for convenience: 

    // This converts any value of type T into a Result<T> containing that value
    //      Takes any object of type T (like Product, string, int, etc.)
    //          => return Result<Product>.Ok(product);
    //      Automatically wraps it in Result<T>.Ok(value)
    //          => return product;
    public static implicit operator ResultOfT<T>(T value) => Ok(value);

    // This converts an Error object into a Result<T> containing that error
    //      Takes any Error object
    //          => return Result<Product>.BadRequest(ProductErrors.InvalidName);
    //      Automatically wraps it in Result<T>.BadRequest(error)
    //          => return ProductErrors.InvalidName;
    //      This means you can return an error directly and it becomes a failed result
    public static implicit operator ResultOfT<T>(Error error)
    {
        return error.Type switch
        {
            ErrorTypes.Validation => BadRequest(error),
            ErrorTypes.NotFound => NotFound(error),
            ErrorTypes.Conflict => Conflict(error),
            ErrorTypes.UnAuthorized => Unauthorized(error),
            ErrorTypes.Forbidden => Forbidden(error),
            ErrorTypes.Failure => Failure(error),
            _ => BadRequest(error)
        };
    }
}
