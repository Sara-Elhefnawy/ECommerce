using ECommerce.APP.Types.Response;
using ECommerce.APP.Types.Specifications;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Types.Queries;

public sealed class DetailsTypeQuery(IReadRepository<ProductType> repository)
{
    public async Task<ResultOfT<DetailsTypeResponse>> Execute(Guid id, CancellationToken ct = default)
    {
        var type = await repository.FirstOrDefaultAsync(new DetailsTypeSpecification(id), ct);

        return type is null
            ? ProductErrors.NotFound
            : type;
    }
}
