using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Queries.GetAll;

public sealed class GetAllDeliveryMethodsHandler(
    IReadRepository<DeliveryMethod> repository) 
    : IRequestHandler<GetAllDeliveryMethodsQuery, ResultOfT<IReadOnlyList<DeliveryMethodResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<DeliveryMethodResponse>>> Handle(GetAllDeliveryMethodsQuery request, CancellationToken ct = default)
    {
        var deliveryMethods = await repository.ListAsync(new GetAllDeliveryMethodsSpecification(request.AvailableOnly, request.SearchTerm), ct);

        return ResultOfT<IReadOnlyList<DeliveryMethodResponse>>.Ok(deliveryMethods);
    }
}
