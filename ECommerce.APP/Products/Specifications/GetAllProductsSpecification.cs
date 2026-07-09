using ECommerce.APP.Products.Responses;
using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Specifications;

public sealed class GetAllProductsSpecification : Specification<Product, GetAllProductsResponse>
{
    public GetAllProductsSpecification()
    {
        Query.OrderBy(p => p.Name)
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
