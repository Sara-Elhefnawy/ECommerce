using ECommerce.Domain.Common;

namespace ECommerce.APP.Abstractions.Mediator;

/// <summary>
/// Creates a validation failure for either: Result || ResultOfT
///
/// ValidationBehavior only knows TResponse,
/// so reflection is used to call the correct
/// static BadRequest(...) factory.
/// </summary>
internal static class ResultFactory
{
    public static TResponse CreateValidationFailure<TResponse>(Error error)
    {
        var responseType = typeof(TResponse);

        // Non generic Result
        if (responseType == typeof(Result))
            return (TResponse)(object)Result.BadRequest(error);

        // ResultOfT<T>
        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(ResultOfT<>))
        {
            var method = responseType.GetMethod(nameof(ResultOfT<object>.BadRequest));

            return (TResponse)method!.Invoke(null, new object[] { error })!;
        }

        // ValidationBehavior only supports Result and ResultOfT<T>.
        throw new InvalidOperationException(
            $"'{responseType.Name}' is not a supported response type.");
    }
}
