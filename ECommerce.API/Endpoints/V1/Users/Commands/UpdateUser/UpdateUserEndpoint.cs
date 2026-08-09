using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Users;
using ECommerce.APP.Features.Users.Commands.UpdateUser;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.Users.Commands.UpdateUser;

public sealed class UpdateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
     => app.MapVersionedEndpoint("users", ApiVersions.V1)
            .MapPut("/me", Handle)
            .WithTags("Users")
            .WithName("Update user")
            .WithGroupName("v1")
            .Produces<ApiResponse<UserProfileResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Updates the authenticated user's profile.")
            .WithSummary("Update current user profile")
            .RequireAuthorization(policy => policy.RequireRole(Roles.User));

    public static async Task<IResult> Handle(
        UpdateUserCommand command,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Current user is updated successfully");
    }
}
