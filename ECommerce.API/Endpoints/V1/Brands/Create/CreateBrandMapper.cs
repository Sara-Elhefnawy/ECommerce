using ECommerce.APP.Features.Brands.Commands;

namespace ECommerce.API.Endpoints.V1.Brands.Create;

public static class CreateBrandMapper
{
    public static CreateBrandCommand ToCommand(this CreateBrandRequest request)
        => new(request.Name);
}
