using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Auth;
using ECommerce.APP.Features.Auth.Commands.ResendVerification;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Auth.ResendVerification;

public sealed class ResendVerificationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("auth", ApiVersions.V1)
            .MapPost("/resend-verification", Handle)
            .WithTags("Auth")
            .WithName("Resend Verification")
            .WithGroupName("v1")
            .Produces<ApiResponse<EmailSentResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
            .WithDescription("Resend the verification code. Does not return a JWT.")
            .WithSummary("Resend the verification code")
            .AllowAnonymous();

    public static async Task<IResult> Handle(
        [AsParameters] ResendVerificationCommand command,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Resent the verification code successfully");
    }
}
