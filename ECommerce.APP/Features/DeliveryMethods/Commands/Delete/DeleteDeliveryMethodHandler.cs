using ECommerce.APP.Features.DeliveryMethods.Queries.GetById;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Delete;

public sealed class DeleteDeliveryMethodHandler(
    IRepository<DeliveryMethod> repository,
    IUnitOfWork uow)
    : IRequestHandler<DeleteDeliveryMethodCommand, Result>
{
    public async Task<Result> Handle(
        DeleteDeliveryMethodCommand request,
        CancellationToken ct = default)
    {
        var entity = await repository.FirstOrDefaultAsync(new GetDeliveryMethodEntityByIdSpecification(request.Id), ct);

        if (entity is null)
            return DeliveryMethodErrors.NotFound;

        repository.SoftDelete(entity);
        await uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
