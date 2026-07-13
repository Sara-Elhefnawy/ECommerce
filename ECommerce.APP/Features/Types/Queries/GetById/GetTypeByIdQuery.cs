using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Types.Queries.GetById;

public sealed record GetTypeByIdQuery(Guid Id) : IRequest<ResultOfT<GetTypeByIdResponse>>;
