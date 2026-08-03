using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Auth;
using ECommerce.APP.Features.Auth.Commands.Login;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Auth.Login;

public sealed class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("auth", ApiVersions.V1)
            .MapPost("/login", Handle)
            .WithTags("Auth")
            .WithName("Login")
            .WithGroupName("v1")
            .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .WithDescription("Validates the credentials and returns access + refresh tokens for a confirmed account.")
            .WithSummary("Login with email and password")
            .AllowAnonymous();

    public static async Task<IResult> Handle(
        [AsParameters] LoginCommand command,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Login successful");
    }
}
