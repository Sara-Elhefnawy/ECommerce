using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Auth.Queries.GetRoles;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.Auth.GetRoles;

public sealed class GetRolesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("auth", ApiVersions.V1)
            .MapGet("/users/{userId:guid}/roles", Handle)
            .WithTags("Auth")
            .WithName("Role retrieval")
            .WithGroupName("v1")
            .Produces<ApiResponse<GetRolesResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithDescription("Retrieve role to Super Admins.")
            .WithSummary("Retrieve role to Super Admins")
            .RequireAuthorization(policy => policy.RequireRole(Roles.SuperAdmin));

    public async Task<IResult> Handle(
        Guid userId,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetRolesQuery(userId), ct);

        return result.ToApiResult(httpContext, "Retrieved role to user successfully");
    }
}
