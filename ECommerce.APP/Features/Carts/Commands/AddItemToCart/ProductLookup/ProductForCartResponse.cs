namespace ECommerce.APP.Features.Carts.Commands.AddItemToCart.ProductLookup;

public record ProductForCartResponse(Guid Id, string Name, string PictureUrl, decimal Price);
