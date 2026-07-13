using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Types.Queries.GetByName;

public sealed record GetTypeByNameQuery(string Name) : IRequest<ResultOfT<GetTypeByNameResponse>>;
