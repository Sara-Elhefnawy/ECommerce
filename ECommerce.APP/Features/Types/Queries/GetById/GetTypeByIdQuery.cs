using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Types.Queries.GetById;

public sealed record GetTypeByIdQuery(Guid Id) : IRequest<ResultOfT<GetTypeByIdResponse>>;
