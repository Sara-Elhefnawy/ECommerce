using ECommerce.APP.Features.Brands.Commands;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.API.Endpoints.V1.Brands.Create;

public sealed class CreateBrandRequest : IRequest<ResultOfT<CreateBrandResponse>>
{
    public required string Name { get; set; } = default!;
}
