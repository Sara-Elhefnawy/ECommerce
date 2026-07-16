using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Products.Queries.GetAll;

public sealed class GetAllProductsSpecification : Specification<Product, GetAllProductsResponse>
{
    public GetAllProductsSpecification(int? count)
    {
        Query.OrderBy(p => p.Name);

        if (count is int value)
            Query.Take(count.Value);

        Query
            .Select(p => new GetAllProductsResponse
            (
                p.Id,
                p.Name,
                p.Description,
                p.PictureUrl,
                p.Price,
                p.Type.Name,
                p.Brand.Name
            ));
    }
}
