using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Users;
using ECommerce.APP.Features.Users.Queries.GetUserAddresses;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.Users.Queries.GetUserAddresses;

public sealed class GetUserAddressesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("users", ApiVersions.V1)
            .MapGet("/me/addresses", Handle)
            .WithTags("Users")
            .WithName("Get user's addresses")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<UserAddressResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithDescription("Get the authenticated user's addresses.")
            .WithSummary("Get current user addresses")
            .RequireAuthorization(policy => policy.RequireRole(Roles.User));

    public static async Task<IResult> Handle(
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetUserAddressesQuery(), ct);

        return result.ToApiResult(httpContext, "Current user's addresses are retrieved successfully");
    }
}
