using FluentValidation;
using ECommerce.Domain.Common;
using ECommerce.APP.Mediator;

namespace ECommerce.APP.Mediator.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var failures = validators
            .Select(v => v.Validate(request))
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var error = Error.Validation(
            "Validation.Failed",
            string.Join("; ", failures.Select(f => f.ErrorMessage)));

        return ResultFactory.CreateValidationFailure<TResponse>(error);
    }
}
