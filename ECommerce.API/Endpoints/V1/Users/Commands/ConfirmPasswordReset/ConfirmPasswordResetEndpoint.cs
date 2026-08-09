using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Users.Commands.ConfirmPasswordReset;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.API.Endpoints.V1.Users.Commands.ConfirmPasswordReset;

public sealed class ConfirmPasswordResetEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("users", ApiVersions.V1)
            .MapPost("/confirm-reset-password", Handle)
            .WithTags("Users")
            .WithName("ConfirmResetPassword")
            .WithGroupName("v1")
            .Produces<ResultOfT<ConfirmPasswordResetResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDescription("Validates the password reset token and updates the user's password")
            .WithSummary("Validates the password reset token and updates the user's password");

    public static async Task<IResult> Handle(
        ConfirmPasswordResetCommand command,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Password has been reset successfully.");
    }
}
