using ECommerce.APP.Brands.Queries;
using ECommerce.APP.Products.Responses;
using ECommerce.APP.Products.Responses.Extensions;
using ECommerce.APP.Types.Queries;
using ECommerce.Domain.Abstractions.Cloudinaryy;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Commands;

public sealed class CreateProductCommand(
    IUnitOfWork uow,
    DetailsBrandQuery brandQuery,
    DetailsTypeQuery typeQuery,
    ICloudinaryService cloudinaryService)
{
    public async Task<ResultOfT<CreateProductResponse>> Execute(
        CreateProductRequest request,
        CancellationToken ct = default)
    {
        // Validate image using domain rules
        var imageValidation = Product.ValidateImageFile(request.ImageFile);
        if (imageValidation.IsFailure)
            return imageValidation.Error!;

        var brandExists = await brandQuery.Execute(request.BrandId, ct);
        if (brandExists.IsFailure)
            return ProductErrors.InvalidBrand;

        var typeExists = await typeQuery.Execute(request.TypeId, ct);
        if (typeExists.IsFailure)
            return ProductErrors.InvalidType;

        // Upload image to Cloudinary
        string imageUrl;
        using (var stream = request.ImageFile.OpenReadStream())
        {
            imageUrl = await cloudinaryService.UploadImageAsync(stream, request.ImageFile.FileName, ct);
        }

        var result = Product.Create(
            request.Name, request.Description, imageUrl,
            request.Price, request.BrandId, request.TypeId);

        if (result.IsFailure)
            return result.Error!;

        var productRepo = uow.Repository<Product>();
        productRepo.Add(result.Value);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<CreateProductResponse>.Created(result.Value.ToCreateResponse());
    }
}
