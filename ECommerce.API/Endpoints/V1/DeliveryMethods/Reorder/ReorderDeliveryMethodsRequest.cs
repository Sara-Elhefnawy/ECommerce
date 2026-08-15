namespace ECommerce.API.Endpoints.V1.DeliveryMethods.Reorder;

public sealed record ReorderDeliveryMethodsRequest(IReadOnlyList<string> DeliveryMethodIds);
