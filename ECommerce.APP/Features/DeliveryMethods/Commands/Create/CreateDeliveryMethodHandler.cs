using ECommerce.APP.Features.DeliveryMethods.Queries.GetByName;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Create;

public sealed class CreateDeliveryMethodHandler(IUnitOfWork uow)
    : IRequestHandler<CreateDeliveryMethodCommand, ResultOfT<DeliveryMethodResponse>>
{
    public async Task<ResultOfT<DeliveryMethodResponse>> Handle(
    CreateDeliveryMethodCommand request,
    CancellationToken ct = default)
    {
        var repo = uow.Repository<DeliveryMethod>();

        var existing = await repo.FirstOrDefaultAsync(new GetDeliveryMethodByNameSpecification(request.Name), ct);

        if (existing is not null)
            return DeliveryMethodErrors.NameAlreadyExists;

        var methods = await repo.ListAsync(new CreateDeliveryMethodSpecification(), ct);

        var nextDisplayOrder =
            methods.Count == 0
                ? 1
                : methods.Max(x => x.DisplayOrder) + 1;

        var result = DeliveryMethod.Create(
            request.Name,
            request.Price,
            request.EstimatedDeliveryTime,
            request.Description,
            request.IsAvailable,
            nextDisplayOrder
            );

        if (result.IsFailure)
            return result.Error!;

        repo.Add(result.Value);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<DeliveryMethodResponse>.Created(
            new DeliveryMethodResponse(
                result.Value.Id,
                result.Value.Name,
                result.Value.Description,
                result.Value.Price,
                result.Value.EstimatedDeliveryTime,
                result.Value.IsAvailable,
                result.Value.DisplayOrder));
    }
}
