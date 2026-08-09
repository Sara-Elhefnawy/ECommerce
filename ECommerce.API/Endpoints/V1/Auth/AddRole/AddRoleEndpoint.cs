using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Auth.Commands.AddRole;
using ECommerce.APP.Mediator;
using ECommerce.APP.Token;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.Auth.AddRole;

public sealed class AddRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("auth", ApiVersions.V1)
            .MapPost("/users/{userId:guid}/roles", Handle)
            .WithTags("Auth")
            .WithName("Role addition")
            .WithGroupName("v1")
            .Produces<ApiResponse<AuthUserSnapshot>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithDescription("Adds role to authenticated user.")
            .WithSummary("Adds role to authenticated user")
            .RequireAuthorization(policy => policy.RequireRole(Roles.SuperAdmin));

    public async Task<IResult> Handle(
        Guid userId,
        AddRoleRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        // No ClaimsPrincipal needed here anymore — RequireAuthorization already
        // confirmed the CALLER is a SuperAdmin. userId is who the action applies to.
        var result = await mediator.Send(new AddRoleCommand(userId, request.Role), ct);

        return result.ToApiResult(httpContext, $"Added role '{request.Role}' to user successfully");
    }
}
