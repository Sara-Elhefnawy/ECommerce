using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Errors;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Types.Queries.GetById;

public sealed class GetTypeByIdHandler(IReadRepository<ProductType> repository) :

    IRequestHandler<GetTypeByIdQuery, ResultOfT<GetTypeByIdResponse>>
{
    public async Task<ResultOfT<GetTypeByIdResponse>> Handle(
        GetTypeByIdQuery request, CancellationToken ct = default)
    {
        var type = await repository.FirstOrDefaultAsync(new GetTypeByIdSpecification(request.Id), ct);

        return type is null
            ? TypeErrors.NotFound
            : type;
    }
}
