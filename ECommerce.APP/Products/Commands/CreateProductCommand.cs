using ECommerce.APP.Products.Responses;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Commands;

public sealed class CreateProductCommand(IUnitOfWork uow)
{
    public async Task<ResultOfT<Product>> Execute(CreateProductRequest request, CancellationToken ct = default)
    {
        var brandExists = await uow.Repository<ProductBrand>().GetByIdAsync(request.BrandId, ct) is not null;
        if (!brandExists)
            return ProductErrors.InvalidBrand;

        var typeExists = await uow.Repository<ProductType>().GetByIdAsync(request.TypeId, ct) is not null;
        if (!typeExists)
            return ProductErrors.InvalidType;

        var result = Product.Create(
            request.Name, request.Description, request.PictureUrl,
            request.Price, request.BrandId, request.TypeId);

        if (result.IsFailure)
            return result.Error!;

        var productRepo = uow.Repository<Product>();
        productRepo.Add(result.Value);
        await uow.SaveChangesAsync(ct);

        return result.Value;
    }
}
