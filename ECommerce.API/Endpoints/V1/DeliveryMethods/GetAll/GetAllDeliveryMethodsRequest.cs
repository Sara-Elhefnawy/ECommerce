namespace ECommerce.API.Endpoints.V1.DeliveryMethods.GetAll;

public sealed record GetAllDeliveryMethodsRequest(bool AvailableOnly = true, string? SearchTerm = null);
