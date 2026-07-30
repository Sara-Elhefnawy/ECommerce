using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Token;

namespace ECommerce.API.Endpoints.V1.Test;

public class TestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("tests", ApiVersions.V1)
            .MapGet("/jwt", Handle)
            .WithTags("Test")
            .WithName("TestAccessToken")
            .WithGroupName("v1")
            .WithSummary("Test access token")
            .AllowAnonymous();

    public static async Task<IResult> Handle(
        [AsParameters] GenerateTestJwtRequest request,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        var token = jwtTokenGenerator.GenerateToken(
            request.UserId,
            request.Email,
            request.DisplayName,
            request.Roles);

        return Results.Ok(token);
    }
}
