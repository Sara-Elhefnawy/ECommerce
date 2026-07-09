using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Types.Queries.Details;

public sealed class DetailsTypeHandler(IReadRepository<ProductType> repository) :

    IRequestHandler<DetailsTypeQuery, ResultOfT<DetailsTypeResponse>>
{
    public async Task<ResultOfT<DetailsTypeResponse>> Handle(
        DetailsTypeQuery request, CancellationToken ct = default)
    {
        var type = await repository.FirstOrDefaultAsync(new DetailsTypeSpecification(request.Id), ct);

        return type is null
            ? ProductErrors.NotFound
            : type;
    }
}
