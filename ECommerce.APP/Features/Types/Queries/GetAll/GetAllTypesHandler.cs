using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Types.Queries.GetAll;

public sealed class GetAllTypesHandler(IReadRepository<ProductType> repository) :
    IRequestHandler<GetAllTypesQuery, ResultOfT<IReadOnlyList<GetAllTypesResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<GetAllTypesResponse>>> Handle(
        GetAllTypesQuery request, CancellationToken ct)
    {
        if (request.Count < 0 || request.Count > 50)
            return ResultOfT<IReadOnlyList<GetAllTypesResponse>>.BadRequest(TypeErrors.InvalidCount);

        var types = await repository.ListAsync(new GetAllTypesSpecification(request.Count), ct);

        return ResultOfT<IReadOnlyList<GetAllTypesResponse>>.Ok(types);
    }
}
