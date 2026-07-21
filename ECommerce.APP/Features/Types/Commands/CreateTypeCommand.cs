using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Types.Commands;

public sealed record CreateTypeCommand(string Name) : IRequest<ResultOfT<CreateTypeResponse>>;
