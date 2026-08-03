using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Auth;
using ECommerce.APP.Features.Auth.Commands.Register;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Auth.Register;

public sealed class RegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("auth", ApiVersions.V1)
            .MapGet("/register", Handle)
            .WithTags("Auth")
            .WithName("Registeration")
            .WithGroupName("v1")
            .Produces<ApiResponse<EmailSentResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithDescription("Creates an unconfirmed account and emails a verification code. Does not return a JWT.")
            .WithSummary("Register a new user")
            .AllowAnonymous();

    public static async Task<IResult> Handle(
        [AsParameters] RegisterCommand command,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Created a new user successfully");
    }
}
