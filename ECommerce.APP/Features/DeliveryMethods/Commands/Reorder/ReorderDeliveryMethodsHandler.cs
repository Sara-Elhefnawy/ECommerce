using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Reorder;

public sealed class ReorderDeliveryMethodsHandler(
    IRepository<DeliveryMethod> repository,
    IUnitOfWork uow)
    : IRequestHandler<ReorderDeliveryMethodsCommand, Result>
{
    public async Task<Result> Handle(
        ReorderDeliveryMethodsCommand request,
        CancellationToken ct = default)
    {
        var deliveryMethods = await repository.ListAsync(new ReorderDeliveryMethodsSpecification(request.DeliveryMethodIds), ct);

        if (deliveryMethods.Count != request.DeliveryMethodIds.Count)
            return DeliveryMethodErrors.NotFound;

        var methodsById = deliveryMethods.ToDictionary(x => x.Id);

        for (var index = 0; index < request.DeliveryMethodIds.Count; index++)
        {
            var id = request.DeliveryMethodIds[index];

            var deliveryMethod = methodsById[id];

            var newDisplayOrder = index + 1;

            if (deliveryMethod.DisplayOrder != newDisplayOrder)
            {
                deliveryMethod.SetDisplayOrder(newDisplayOrder);
                repository.Update(deliveryMethod);
            }
        }

        await uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
