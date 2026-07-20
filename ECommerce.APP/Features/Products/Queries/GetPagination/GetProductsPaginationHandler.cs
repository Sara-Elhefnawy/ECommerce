using ECommerce.APP.Features.Products.Queries.GetAll;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Products.Queries.GetPagination;

public sealed class GetProductsPaginationHandler(IReadRepository<Product> repository) :
    IRequestHandler<GetProductsPaginationQuery, ResultOfT<PagedResult<GetAllProductsResponse>>>
{
    public async Task<ResultOfT<PagedResult<GetAllProductsResponse>>> Handle(GetProductsPaginationQuery request, CancellationToken ct = default)
    {
        var countSpec = new GetProductsPaginationSpecification(request.SearchTerm, request.BrandId, request.TypeId);

        var listSpec = new GetProductsPaginationSpecification(request.SearchTerm, request.BrandId, request.TypeId, request.SortBy, request.IsSortDescending, request.PageSize, request.PageNumber);

        var count = await repository.CountAsync(countSpec, ct);

        var items = await repository.ListAsync(listSpec, ct);

        return ResultOfT<PagedResult<GetAllProductsResponse>>.Ok(new PagedResult<GetAllProductsResponse>(items, count, request.PageNumber, request.PageSize));
    }
}
