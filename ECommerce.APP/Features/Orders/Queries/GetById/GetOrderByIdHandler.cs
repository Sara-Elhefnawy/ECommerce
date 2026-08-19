using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Features.Orders.Mapper;
using ECommerce.APP.Features.Orders.Specifications;
using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Orders.Queries.GetById;

public sealed class GetOrderByIdHandler(
    ICurrentUserService currentUser,
    IReadRepository<Order> orderRepository)
    : IRequestHandler<GetOrderByIdQuery, ResultOfT<OrderResponse>>
{
    public async Task<ResultOfT<OrderResponse>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
            return OrderErrors.Unauthorized;

        var order = await orderRepository.FirstOrDefaultAsync(
            new GetOrderByIdSpecification(request.OrderId, currentUser.UserId.Value),
            cancellationToken);

        if (order is null)
            return OrderErrors.NotFound;

        return OrderMapper.ToResponse(order);
    }
}
