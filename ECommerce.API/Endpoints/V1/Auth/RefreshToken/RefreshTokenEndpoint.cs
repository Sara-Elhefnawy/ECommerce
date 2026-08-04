using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Auth;
using ECommerce.APP.Features.Auth.Commands.RefreshTokens;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Auth.RefreshToken;

public sealed class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("auth", ApiVersions.V1)
            .MapPost("/refresh", Handle)
            .WithTags("Auth")
            .WithName("Refresh Token")
            .WithGroupName("v1")
            .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithDescription("Exchanges a valid refresh token for a new access token and rotated refresh token.")
            .WithSummary("Refresh tokens")
            .AllowAnonymous();

    public static async Task<IResult> Handle(
        RefreshTokenCommand command,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Token refreshed successful");
    }
}
