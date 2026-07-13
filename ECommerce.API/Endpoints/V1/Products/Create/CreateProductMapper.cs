using ECommerce.APP.Features.Products.Commands;

namespace ECommerce.API.Endpoints.V1.Products.Create;

public static class CreateProductMapper
{
    public static CreateProductCommand ToCommand(this CreateProductRequest request)
        => new(
            request.Name, 
            request.Description, 
            request.Price,
            request.BrandName, 
            request.TypeName,
            request.ImageFile.OpenReadStream(),
            request.ImageFile.FileName,
            request.ImageFile.Length,
            request.ImageFile.ContentType);
}
