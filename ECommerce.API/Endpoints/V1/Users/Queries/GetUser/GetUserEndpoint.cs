using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Users;
using ECommerce.APP.Features.Users.Queries.GetUser;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Users.Queries.GetUser;

public sealed class GetUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
     => app.MapVersionedEndpoint("users", ApiVersions.V1)
            .MapGet("/me", Handle)
            .WithTags("Users")
            .WithName("Get user")
            .WithGroupName("v1")
            .Produces<ApiResponse<UserProfileResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithDescription("Returns the authenticated user's profile.")
            .WithSummary("Get current user")
            .RequireAuthorization();

    public static async Task<IResult> Handle(
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetCurrentUserQuery(), ct);

        return result.ToApiResult(httpContext, "Current user retrieved successfully");
    }
}
