using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Brands.Commands;

public sealed class CreateBrandHandler(IUnitOfWork uow) :
    IRequestHandler<CreateBrandCommand, ResultOfT<CreateBrandResponse>>
{
    public async Task<ResultOfT<CreateBrandResponse>> Handle(
        CreateBrandCommand request,
        CancellationToken ct = default)
    {
        var result = ProductBrand.Create(request.Name);

        if (result.IsFailure)
            return result.Error!;

        var brandRepo = uow.Repository<ProductBrand>();
        brandRepo.Add(result.Value);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<CreateBrandResponse>.Created(
            new CreateBrandResponse(
                result.Value.Id,
                result.Value.Name
            ));
    }
}
