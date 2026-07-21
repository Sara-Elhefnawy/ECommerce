using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Carts.Queries.GetCart;

public sealed record GetCartQuery(Guid BuyerId) : IRequest<ResultOfT<GetCartResponse>>;
