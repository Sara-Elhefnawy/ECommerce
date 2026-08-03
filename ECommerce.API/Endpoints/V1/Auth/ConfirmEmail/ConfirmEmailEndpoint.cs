using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Auth;
using ECommerce.APP.Features.Auth.Commands.ConfirmEmail;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Auth.ConfirmEmail;

public sealed class ConfirmEmailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("auth", ApiVersions.V1)
            .MapGet("/confirm-email", Handle)
            .WithTags("Auth")
            .WithName("Email Confirmation")
            .WithGroupName("v1")
            .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .WithDescription("Validates the code and returns access + refresh tokens.")
            .WithSummary("Confirm email with verification code")
            .RequireRateLimiting("verify-code")
            .AllowAnonymous();

    public static async Task<IResult> Handle(
        [AsParameters] ConfirmEmailCommand command,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Email confirmed successfully");
    }
}
