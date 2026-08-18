using ECommerce.APP.Features.Brands.Queries.GetByName;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Brands.Commands;

public sealed class CreateBrandHandler(IUnitOfWork uow) :
    IRequestHandler<CreateBrandCommand, ResultOfT<CreateBrandResponse>>
{
    public async Task<ResultOfT<CreateBrandResponse>> Handle(
        CreateBrandCommand request,
        CancellationToken ct = default)
    {
        var brandRepo = uow.Repository<ProductBrand>();

        var brand = await brandRepo.FirstOrDefaultAsync(new GetBrandByNameSpecification(request.Name), ct);

        if (brand is not null)
            return BrandErrors.AlreadyExists;

        var result = ProductBrand.Create(request.Name);

        if (result.IsFailure)
            return result.Error!;

        brandRepo.Add(result.Value);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<CreateBrandResponse>.Created(
            new CreateBrandResponse(
                result.Value.Id,
                result.Value.Name
            ));
    }
}
