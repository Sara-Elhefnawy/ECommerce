using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Brands.Queries.GetByName;

public class GetBrandByNameHandler(IReadRepository<ProductBrand> repository) : IRequestHandler<GetBrandByNameQuery, ResultOfT<GetBrandByNameResponse>>
{
    public async Task<ResultOfT<GetBrandByNameResponse>> Handle(
        GetBrandByNameQuery request, 
        CancellationToken ct = default)
    {
        var brand = await repository.FirstOrDefaultAsync(new GetBrandByNameSpecification(request.Name.ToUpperInvariant()), ct);

        if (brand is null)
            return ProductErrors.InvalidBrand;

        return ResultOfT<GetBrandByNameResponse>.Ok(brand);
    }
}
