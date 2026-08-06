using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Auth.Commands.RemoveRole;
using ECommerce.APP.Mediator;
using ECommerce.APP.Token;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints.V1.Auth.RemoveRole;

public sealed class RemoveRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("auth", ApiVersions.V1)
            .MapDelete("/remove-role", Handle)
            .WithTags("Auth")
            .WithName("Role removal")
            .WithGroupName("v1")
            .Produces<ApiResponse<AuthUserSnapshot>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithDescription("Removes role from authenticated user.")
            .WithSummary("Removes role from authenticated user")
            .RequireAuthorization();

    public async Task<IResult> Handle(
        [FromBody] RemoveRoleCommand request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(request, ct);

        return result.ToApiResult(httpContext, "Role removed from user successfully.");
    }
}
