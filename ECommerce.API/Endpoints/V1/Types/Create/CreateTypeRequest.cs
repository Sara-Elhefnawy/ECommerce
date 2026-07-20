using ECommerce.APP.Features.Types.Commands;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.API.Endpoints.V1.Types.Create;

public sealed class CreateTypeRequest : IRequest<ResultOfT<CreateTypeResponse>>
{
    public required string Name { get; set; } = default!;
}
