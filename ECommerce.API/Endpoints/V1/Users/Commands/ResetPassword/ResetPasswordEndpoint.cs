using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Users.Commands.ResetPassword;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Users.Commands.ResetPassword;

public sealed class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("users", ApiVersions.V1)
            .MapPost("/reset-password", Handle)
            .WithTags("Users")
            .WithName("ResetPassword")
            .WithGroupName("v1")
            .Produces<ResetPasswordResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDescription("Resets the user's password using a reset token.")
            .WithSummary("Reset password");

    public static async Task<IResult> Handle(
        ResetPasswordCommand command,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "If that email exists, a password reset link has been sent.");
    }
}
