using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Brands.Queries.GetByName;

public class GetBrandByNameHandler(IReadRepository<ProductBrand> repository) : IRequestHandler<GetBrandByNameQuery, ResultOfT<GetBrandByNameResponse>>
{
    public async Task<ResultOfT<GetBrandByNameResponse>> Handle(
        GetBrandByNameQuery request, 
        CancellationToken ct = default)
    {
        var brand = await repository.FirstOrDefaultAsync(new GetBrandByNameSpecification(request.Name), ct);

        return brand is null ? BrandErrors.NotFound : brand;
    }
}
