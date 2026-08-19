using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;
using FluentValidation;

namespace ECommerce.API.Endpoints.V1.Orders.GetUserOrders;

public sealed class GetCurrentUserOrdersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("orders", ApiVersions.V1)
            .MapGet("/", Handle)
            .WithTags("Orders")
            .WithName("getUserOrders")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<OrderResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get current user orders")
            .WithDescription("get the authenticated user's orders.")
            .RequireAuthorization(policy => policy.RequireRole(Roles.User));

    public static async Task<IResult> Handle(
        [AsParameters] GetCurrentUserOrdersRequest request,
        IValidator<GetCurrentUserOrdersRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(request, ct);

        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var query = request.ToQuery();

        var result = await mediator.Send(query, ct);

        return result.ToPaginatedApiResult(httpContext, request.PageNumber, request.PageSize, "Retrieved paginated orders successfully");
    }
}
