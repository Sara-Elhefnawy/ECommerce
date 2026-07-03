using ECommerce.APP.Products.Responses;

namespace ECommerce.APP.Products;

public interface IProductQueryService
{
    Task<IReadOnlyList<GetAllProductsResponse>> GetAllAsync(CancellationToken ct = default);
   
    Task<DetailsProductResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<GetAllProductsResponse>?> GetByBrandIdAsync(Guid brandId, CancellationToken ct = default);

    Task<IReadOnlyList<GetAllProductsResponse>?> GetByTypeIdAsync(Guid typeId, CancellationToken ct = default);

    Task CreateAsync(CreateProductRequest request, CancellationToken ct = default);
}
