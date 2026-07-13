using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Products.Queries.GetById;

public sealed class GetProductByIdSpecification : Specification<Product, GetProductByIdResponse>
{
    public GetProductByIdSpecification(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            .Select(p => new GetProductByIdResponse(
                p.Id,
                p.Name,
                p.Description,
                p.PictureUrl,
                p.Price,
                p.Type.Name,
                p.Brand.Name));
    }
}
