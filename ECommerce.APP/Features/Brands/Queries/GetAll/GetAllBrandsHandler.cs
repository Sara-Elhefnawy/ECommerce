using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Brands.Queries.GetAll;

public sealed class GetAllBrandsHandler(IReadRepository<ProductBrand> repository) : 
    IRequestHandler<GetAllBrandsQuery, ResultOfT<IReadOnlyList<GetAllBrandsResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<GetAllBrandsResponse>>> Handle(
        GetAllBrandsQuery request, CancellationToken ct)
    {
        if (request.Count < 0 || request.Count > 50)
            return ResultOfT<IReadOnlyList<GetAllBrandsResponse>>.BadRequest(BrandErrors.InvalidCount);

        var brands = await repository.ListAsync(new GetAllBrandsSpecification(request.Count), ct);

        return ResultOfT<IReadOnlyList<GetAllBrandsResponse>>.Ok(brands);
    }
}
