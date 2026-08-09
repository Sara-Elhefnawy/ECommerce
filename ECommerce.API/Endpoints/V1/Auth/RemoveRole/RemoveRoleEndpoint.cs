using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Auth.Commands.RemoveRole;
using ECommerce.APP.Mediator;
using ECommerce.APP.Token;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.Auth.RemoveRole;

public sealed class RemoveRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("auth", ApiVersions.V1)
            .MapDelete("/users/{userId:guid}/roles/{role}", Handle)
            .WithTags("Auth")
            .WithName("Role removal")
            .WithGroupName("v1")
            .Produces<ApiResponse<AuthUserSnapshot>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithDescription("Removes role from authenticated user.")
            .WithSummary("Removes role from authenticated user")
            .RequireAuthorization(policy => policy.RequireRole(Roles.SuperAdmin));

    public async Task<IResult> Handle(
        Guid userId,
        string role,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new RemoveRoleCommand(userId, role), ct);

        return result.ToApiResult(httpContext, $"Role '{role}' removed from user successfully.");
    }
}
