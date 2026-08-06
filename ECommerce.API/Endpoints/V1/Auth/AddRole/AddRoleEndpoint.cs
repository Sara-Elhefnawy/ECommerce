using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Auth.Commands.AddRole;
using ECommerce.APP.Mediator;
using ECommerce.APP.Token;

namespace ECommerce.API.Endpoints.V1.Auth.AddRole;

public sealed class AddRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("auth", ApiVersions.V1)
            .MapPost("/add-role", Handle)
            .WithTags("Auth")
            .WithName("Role addition")
            .WithGroupName("v1")
            .Produces<ApiResponse<AuthUserSnapshot>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithDescription("Adds role to authenticated user.")
            .WithSummary("Adds role to authenticated user")
            .RequireAuthorization();

    public async Task<IResult> Handle(
        AddRoleCommand request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(request, ct);

        return result.ToApiResult(httpContext, "Added role to user successfully");
    }
}
