using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Users.Queries.GetUser;

public sealed record GetCurrentUserQuery : IRequest<ResultOfT<UserProfileResponse>>;
