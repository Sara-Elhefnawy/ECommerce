using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Users;
using ECommerce.APP.Features.Users.Commands.AddUserAddersses;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.Users.Commands.AddUserAddersses;

public sealed class AddUserAddressesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("users", ApiVersions.V1)
            .MapPost("/me/addresses", Handle)
            .WithTags("Users")
            .WithName("Add user's addresses")
            .WithGroupName("v1")
            .Produces<ApiResponse<UserAddressResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Add the authenticated user's addresses.")
            .WithSummary("Add address for current user")
            .RequireAuthorization(policy => policy.RequireRole(Roles.User));

    public static async Task<IResult> Handle(
        AddUserAddressesCommand command,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Current user's Addresses are added successfully");
    }
}
