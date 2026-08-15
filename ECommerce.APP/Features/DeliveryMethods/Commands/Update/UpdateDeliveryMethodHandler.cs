using ECommerce.APP.Features.DeliveryMethods.Queries.GetById;
using ECommerce.APP.Features.DeliveryMethods.Queries.GetByName;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Update;

public sealed class UpdateDeliveryMethodHandler(
    IRepository<DeliveryMethod> repository,
    IUnitOfWork uow)
    : IRequestHandler<UpdateDeliveryMethodCommand, ResultOfT<DeliveryMethodResponse>>
{
    public async Task<ResultOfT<DeliveryMethodResponse>> Handle(
        UpdateDeliveryMethodCommand request,
        CancellationToken ct = default)
    {
        var entity = await repository.FirstOrDefaultAsync(new GetDeliveryMethodEntityByIdSpecification(request.Id), ct);

        if (entity is null)
            return DeliveryMethodErrors.NotFound;

        var duplicate = await repository.AnyAsync(new GetDeliveryMethodByNameSpecification(request.Name.ToUpperInvariant().Trim(), request.Id), ct);

        if (duplicate)
            return DeliveryMethodErrors.NameAlreadyExists;

        var updateResult = entity.Update(
            request.Name,
            request.Price,
            request.EstimatedDeliveryTime,
            request.Description,
            request.IsAvailable);

        if (updateResult.IsFailure)
            return updateResult.Error!;

        repository.Update(entity);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<DeliveryMethodResponse>.Ok(
            new DeliveryMethodResponse(
                entity.Id,
                entity.Name,
                entity.Description,
                entity.Price,
                entity.EstimatedDeliveryTime,
                entity.IsAvailable,
                entity.DisplayOrder));
    }
}
