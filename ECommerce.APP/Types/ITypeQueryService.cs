using ECommerce.APP.Types.Response;

namespace ECommerce.APP.Types;

public interface ITypeQueryService
{
    Task<IReadOnlyList<GetAllTypesResponse>> GetAllAsync(CancellationToken ct = default);
}
