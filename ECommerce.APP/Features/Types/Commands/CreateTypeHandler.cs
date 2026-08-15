using ECommerce.APP.Features.Types.Queries.GetByName;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Types.Commands;

public sealed class CreateTypeHandler(IUnitOfWork uow) :
    IRequestHandler<CreateTypeCommand, ResultOfT<CreateTypeResponse>>
{
    public async Task<ResultOfT<CreateTypeResponse>> Handle(
        CreateTypeCommand request,
        CancellationToken ct = default)
    {
        var typeRepo = uow.Repository<ProductType>();

        var type = await typeRepo.FirstOrDefaultAsync(new GetTypeByNameSpecification(request.Name.ToUpperInvariant().Trim()), ct);

        if (type is not null)
            return TypeErrors.AlreadyExists;

        var result = ProductType.Create(request.Name);

        if (result.IsFailure)
            return result.Error!;

        typeRepo.Add(result.Value);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<CreateTypeResponse>.Created(
            new CreateTypeResponse(
                result.Value.Id,
                result.Value.Name
            ));
    }
}
