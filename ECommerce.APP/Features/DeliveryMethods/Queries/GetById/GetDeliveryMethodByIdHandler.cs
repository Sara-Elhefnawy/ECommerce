using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Queries.GetById;

public sealed class GetDeliveryMethodByIdHandler(
    IReadRepository<DeliveryMethod> repository) 
    : IRequestHandler<GetDeliveryMethodByIdQuery, ResultOfT<DeliveryMethodResponse>>
{
    public async Task<ResultOfT<DeliveryMethodResponse>> Handle(GetDeliveryMethodByIdQuery request, CancellationToken ct = default)
    {
        var deliveryMethod = await repository.FirstOrDefaultAsync(new GetDeliveryMethodByIdSpecification(request.Id), ct);

        return deliveryMethod is null
            ? DeliveryMethodErrors.NotFound
            : deliveryMethod;
    }
}
