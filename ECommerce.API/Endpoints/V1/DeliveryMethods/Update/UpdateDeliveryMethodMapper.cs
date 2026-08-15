using ECommerce.APP.Features.DeliveryMethods.Commands.Update;

namespace ECommerce.API.Endpoints.V1.DeliveryMethods.Update;

public static class UpdateDeliveryMethodMapper
{
    public static UpdateDeliveryMethodCommand ToCommand(
        this UpdateDeliveryMethodRequest request,
        Guid id)
        => new(
            id,
            request.Name,
            request.Price,
            request.EstimatedDeliveryTime,
            request.Description,
            request.IsAvailable);
}
