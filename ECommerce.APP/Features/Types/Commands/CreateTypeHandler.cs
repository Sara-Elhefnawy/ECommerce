using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Types.Commands;

public sealed class CreateTypeHandler(IUnitOfWork uow) :
    IRequestHandler<CreateTypeCommand, ResultOfT<CreateTypeResponse>>
{
    public async Task<ResultOfT<CreateTypeResponse>> Handle(
        CreateTypeCommand request,
        CancellationToken ct = default)
    {
        var result = ProductType.Create(request.Name);

        if (result.IsFailure)
            return result.Error!;

        var typeRepo = uow.Repository<ProductType>();
        typeRepo.Add(result.Value);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<CreateTypeResponse>.Created(
            new CreateTypeResponse(
                result.Value.Id,
                result.Value.Name
            ));
    }
}
