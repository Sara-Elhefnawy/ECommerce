using ECommerce.APP.Features.Types.Commands;

namespace ECommerce.API.Endpoints.V1.Types.Create;

public static class CreateTypeMapper
{
    public static CreateTypeCommand ToCommand(this CreateTypeRequest request)
        => new(request.Name);
}
