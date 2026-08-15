using ECommerce.APP.Features.DeliveryMethods.Commands.Create;

namespace ECommerce.API.Endpoints.V1.DeliveryMethods.Create;

public static class CreateDeliveryMethodMapper
{
    public static CreateDeliveryMethodCommand ToCommand(this CreateDeliveryMethodRequest request)
        => new(
            request.Name,
            request.Description,
            request.Price,
            request.EstimatedDeliveryTime,
            request.IsAvailable);
}
