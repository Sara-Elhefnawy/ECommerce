using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Errors;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Types.Queries.GetByName;

public class GetTypeByNameHandler(IReadRepository<ProductType> repository) : IRequestHandler<GetTypeByNameQuery, ResultOfT<GetTypeByNameResponse>>
{
    public async Task<ResultOfT<GetTypeByNameResponse>> Handle(
        GetTypeByNameQuery request, 
        CancellationToken ct = default)
    {
        var type = await repository.FirstOrDefaultAsync(new GetTypeByNameSpecification(request.Name.ToUpperInvariant()), ct);

        if (type is null)
            return TypeErrors.NotFound;

        return ResultOfT<GetTypeByNameResponse>.Ok(type);
    }
}
