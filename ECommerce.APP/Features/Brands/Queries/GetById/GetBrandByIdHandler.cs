using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Brands.Queries.GetById;

public sealed class GetBrandByIdHandler(IReadRepository<ProductBrand> repository) :

    IRequestHandler<GetBrandByIdQuery, ResultOfT<GetBrandByIdResponse>>
{
    public async Task<ResultOfT<GetBrandByIdResponse>> Handle(
        GetBrandByIdQuery request, CancellationToken ct = default)
    {
        var brand = await repository.FirstOrDefaultAsync(new GetBrandByIdSpecification(request.Id), ct);

        return brand is null
            ? BrandErrors.NotFound
            : brand;
    }
}
