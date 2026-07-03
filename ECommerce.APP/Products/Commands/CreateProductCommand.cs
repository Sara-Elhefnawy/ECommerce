using ECommerce.APP.Products.Responses;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Commands;

public sealed class CreateProductCommand(IUnitOfWork uow)
{
    public async Task<ResultOfT<Product>> Execute(CreateProductRequest request, CancellationToken ct = default)
    {
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
