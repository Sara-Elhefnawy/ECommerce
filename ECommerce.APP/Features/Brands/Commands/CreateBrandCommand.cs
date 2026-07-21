using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Brands.Commands;

public sealed record CreateBrandCommand(string Name) : IRequest<ResultOfT<CreateBrandResponse>>;
