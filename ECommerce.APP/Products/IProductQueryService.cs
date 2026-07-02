using ECommerce.APP.Products.Responses;

namespace ECommerce.APP.Products;

public interface IProductQueryService
{
    Task<IReadOnlyList<GetAllProductsResponse>> GetAllAsync(CancellationToken ct = default);
   
    Task<DetailsProductResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
