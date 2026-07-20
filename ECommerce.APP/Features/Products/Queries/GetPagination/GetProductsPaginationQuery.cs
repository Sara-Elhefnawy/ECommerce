using ECommerce.APP.Features.Products.Queries.GetAll;
using ECommerce.APP.Features.Products.Queries.GetPagination.Constants;
using ECommerce.APP.Features.Products.Queries.GetPagination.Enums;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Products.Queries.GetPagination;

public sealed record GetProductsPaginationQuery(
    int PageNumber = ValidatorsConstant.DefaultPageNumber,
    int PageSize = ValidatorsConstant.DefaultPageSize,
    string? SearchTerm = null,
    Guid? BrandId = null,
    Guid? TypeId = null,
    SortType? SortBy = null,
    bool IsSortDescending = false
    ) : IRequest<ResultOfT<PagedResult<GetAllProductsResponse>>>;
